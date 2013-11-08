using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using System.Threading;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching.Memcached
{
	class BinaryNode : INode
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();

		private readonly IPEndPoint endpoint;
		private SafeSocket socket;

		private int state;
		private readonly ConcurrentQueue<Data> writeQueue;
		private readonly Queue<Data> readQueue;
		private readonly Queue<Data> bufferQueue;

		private readonly WriteBuffer writeBuffer;
		private readonly ReceiveBuffer readStream;

		private SegmentListCopier currentWriteCopier;
		private BinaryResponse currentResponse;

		public BinaryNode(IPEndPoint endpoint)
		{
			this.endpoint = endpoint;

			this.writeQueue = new ConcurrentQueue<Data>();
			this.readQueue = new Queue<Data>();
			this.bufferQueue = new Queue<Data>();

			this.writeBuffer = new WriteBuffer(SafeSocket.BufferSize);
			this.readStream = new ReceiveBuffer(SafeSocket.BufferSize);
		}

		public IPEndPoint EndPoint { get { return endpoint; } }

		public bool IsAlive
		{
			get { return state == 1; }
			private set
			{
				Interlocked.Exchange(ref state, value ? 1 : 0);
				Thread.MemoryBarrier();
			}
		}

		public void Connect()
		{
			Connect(true, CancellationToken.None);
		}

		public void Connect(bool reset, CancellationToken token)
		{
			if (log.IsDebugEnabled) log.Debug("Connecting node to {0}, will reset write queue: {1}", endpoint, reset);

			RecreateSocket(token);

			Debug.Assert(currentWriteCopier == null);
			Debug.Assert(currentResponse == null);
			Debug.Assert(readQueue.Count == 0);
			Debug.Assert(bufferQueue.Count == 0);

			if (reset)
			{
				writeQueue.Clear();
				writeBuffer.Reset();
			}

			IsAlive = true;
		}

		private void RecreateSocket(CancellationToken token)
		{
			if (socket != null)
				socket.Dispose();

			socket = new SafeSocket(endpoint);
			socket.Connect(token);

			if (!socket.IsAlive) throw new IOException("Could not connect to " + endpoint);
		}

		public void Shutdown()
		{
			IsAlive = false;

			if (socket != null)
			{
				socket.Dispose();
				socket = null;
			}
		}

		public Task Enqueue(IOperation op)
		{
			var tcs = new TaskCompletionSource<bool>();

			if (!IsAlive)
			{
				tcs.SetException(new IOException(endpoint + " is not alive"));
			}
			else
			{
				var data = new Data
				{
					Op = op,
					Task = tcs
				};

				writeQueue.Enqueue(data);
			}

			return tcs.Task;
		}

		public bool Send()
		{
			try
			{
				return IsAlive ? PerformSend() : false;
			}
			catch (Exception e)
			{
				IsAlive = false;
				HandleIOFail(e);
				throw;
			}
		}

		public bool Receive()
		{
			try
			{
				return IsAlive ? PerformReceive() : false;
			}
			catch (Exception e)
			{
				IsAlive = false;
				HandleIOFail(e);
				throw;
			}
		}

		private void HandleIOFail(Exception e)
		{
			while (bufferQueue.Count > 0)
			{
				var data = bufferQueue.Dequeue();
				data.Task.SetException(new IOException("fail fast", e));
			}

			while (readQueue.Count > 0)
			{
				var data = readQueue.Dequeue();
				data.Task.SetException(new IOException("fail fast", e));
			}

			writeBuffer.Reset();
			readStream.Reset();
			currentWriteCopier = null;
			currentResponse = null;
		}

		private bool PerformSend()
		{
			Debug.Assert(IsAlive);

			if (currentWriteCopier != null)
			{
				if (!currentWriteCopier.WriteTo(writeBuffer))
				{
					var data = DequeueWriteOp();
					bufferQueue.Enqueue(data);
					currentWriteCopier = null;

					if (log.IsTraceEnabled) log.Trace("Sent & finished " + data.Op);
				}
			}

			var didNoop = false;

			if (currentWriteCopier == null)
			{
				while (!writeBuffer.IsFull && !writeQueue.IsEmpty)
				{
					var data = PeekWriteOp();
					var request = data.Op.GetRequest();
					var copier = new SegmentListCopier(request.CreateBuffer());

					data.CorrelationId = request.CorrelationId;

					// TODO handle manual NoOp
					if (copier.Length > writeBuffer.Remaining - NoOp.BufferSize)
					{
						IntroduceNoOp();
						didNoop = true;
					}

					if (copier.WriteTo(writeBuffer))
					{
						currentWriteCopier = copier;
						if (log.IsTraceEnabled) log.Trace("Partial send of " + data.Op);
						break;
					}

					DequeueWriteOp();
					bufferQueue.Enqueue(data);

					if (log.IsTraceEnabled) log.Trace("Full send of " + data.Op);
				}
			}

			if (writeBuffer.Position > 0)
			{
				if (!didNoop)
				{
					if (log.IsTraceEnabled) log.Trace("End of writes, still no op.");
					IntroduceNoOp();
				}

				socket.Write(writeBuffer);

				if (bufferQueue.Count > 0) readQueue.Enqueue(bufferQueue);
				if (log.IsTraceEnabled) log.Trace("Flush write buffer");

				return true;
			}

			Debug.Assert(bufferQueue.Count == 0);

			return false;
		}

		private Data PeekWriteOp()
		{
			Data data;

			if (!writeQueue.TryPeek(out data))
				throw new InvalidOperationException("Write queue is empty and should not be.");

			return data;
		}

		private Data DequeueWriteOp()
		{
			Data data;

			if (!writeQueue.TryDequeue(out data))
				throw new InvalidOperationException("Write queue is empty and should not be.");

			return data;
		}

		/// <summary>
		/// Terminates the write buffer with a noop to force a response after a series of Quiet ops.
		/// </summary>
		private void IntroduceNoOp()
		{
			var noop = new NoOp();
			var request = noop.GetRequest();

			var didWrite = writeBuffer.Write(request.CreateBuffer()[0]);
			Debug.Assert(didWrite == NoOp.BufferSize);

			bufferQueue.Enqueue(new Data { CorrelationId = request.CorrelationId, Op = noop });

			if (log.IsTraceEnabled)
				log.Trace("Adding Noop (id: {0}) to the write buffer", request.CorrelationId);
		}

		private bool PerformReceive()
		{
			Debug.Assert(IsAlive);

			if (readQueue.Count == 0) return false;

			//? TODO check for availability?
			if (readStream.EOF) readStream.Fill(socket);

			while (readQueue.Count > 0)
			{
				var response = currentResponse ?? new BinaryResponse();

				if (response.Read(readStream)) // response is not read fully
				{
					currentResponse = response;
					return true;
				}

				currentResponse = null;
				var matching = false;

				while (!matching && readQueue.Count > 0)
				{
					var data = readQueue.Dequeue();
					matching = data.CorrelationId == response.CorrelationId;

					// null is a response to a successful quiet op
					data.Op.ProcessResponse(matching ? response : null); 

					if (data.Task != null)
						data.Task.TrySetResult(true);
				}
			}

			return false;
		}

		private struct Data
		{
			public IOperation Op;
			public uint CorrelationId;
			public TaskCompletionSource<bool> Task;
		}
	}
}

#region [ License information          ]

/* ************************************************************
 *
 *    Copyright (c) Attila Kiskó, enyim.com
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion
