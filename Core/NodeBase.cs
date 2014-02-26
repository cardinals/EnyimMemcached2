using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Enyim.Caching
{
	public abstract class NodeBase : INode
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private static readonly bool LogTraceEnabled = log.IsTraceEnabled;
		private static readonly bool LogDebugEnabled = log.IsDebugEnabled;

		private readonly ICluster owner;
		private readonly IPEndPoint endpoint;
		private readonly IFailurePolicy failurePolicy;

		private readonly ISocket socket;

		private readonly ConcurrentQueue<Data> writeQueue;
		private readonly AdvQueue<Data> readQueue;
		private readonly AdvQueue<Data> bufferQueue;

		private int state;
		private IRequest currentWriteCopier;
		private Data currentWriteOp;
		private IResponse inprogressResponse;

		private bool mustReconnect;

		private ICounter counterEnqueuePerSec;
		private ICounter counterDequeuePerSec;
		private ICounter counterWritePerSec;
		private ICounter counterErrorPerSec;
		private ICounter counterItemCount;
		private ICounter counterQueue;
		private IGauge gaugeSendSpeed;

		protected NodeBase(IPEndPoint endpoint, ICluster owner, IFailurePolicy failurePolicy, ISocket socket)
		{
			counterEnqueuePerSec = Metrics.Meter("node enqueue/sec", endpoint.ToString(), Interval.Seconds);
			counterDequeuePerSec = Metrics.Meter("node dequeue/sec", endpoint.ToString(), Interval.Seconds);
			counterWritePerSec = Metrics.Meter("node write/sec", endpoint.ToString(), Interval.Seconds);
			counterErrorPerSec = Metrics.Meter("node in error/sec", endpoint.ToString(), Interval.Seconds);
			counterItemCount = Metrics.Counter("commands", endpoint.ToString());
			counterQueue = Metrics.Counter("queue length", endpoint.ToString());
			gaugeSendSpeed = Metrics.Gauge("send speed", endpoint.ToString());

			this.owner = owner;
			this.endpoint = endpoint;
			this.socket = socket;
			this.failurePolicy = failurePolicy;

			this.writeQueue = new ConcurrentQueue<Data>();
			this.readQueue = new AdvQueue<Data>();
			this.bufferQueue = new AdvQueue<Data>();

			this.mustReconnect = true;
			IsAlive = true;
		}

		protected WriteBuffer WriteBuffer { get { return socket.WriteBuffer; } }
		public IPEndPoint EndPoint { get { return endpoint; } }

		public bool IsAlive
		{
			get { return state == 1; }
			private set { Interlocked.Exchange(ref state, value ? 1 : 0); }
		}

		public void Connect()
		{
			Connect(true, CancellationToken.None);
		}

		public virtual void Connect(bool reset, CancellationToken token)
		{
			Debug.Assert(currentWriteCopier == null);
			Debug.Assert(inprogressResponse == null);
			Debug.Assert(readQueue.Count == 0);
			Debug.Assert(bufferQueue.Count == 0);

			if (LogDebugEnabled) log.Debug("Connecting node to {0}, will reset write queue: {1}", endpoint, reset);

			socket.Connect(endpoint, token);
			if (reset) writeQueue.Clear();

			mustReconnect = false;
			IsAlive = true;
		}

		public void Shutdown()
		{
			IsAlive = false;

			if (socket != null)
			{
				socket.Dispose();
				//socket = null;
			}
		}

		public virtual Task<IOperation> Enqueue(IOperation op)
		{
			var tcs = new TaskCompletionSource<IOperation>();
			counterItemCount.Increment();

			if (IsAlive)
			{
				writeQueue.Enqueue(new Data { Op = op, Task = tcs });
				counterEnqueuePerSec.Increment();
				counterQueue.Increment();
			}
			else
			{
				counterErrorPerSec.Increment();
				tcs.SetException(new IOException(endpoint + " is not alive"));
			}

			return tcs.Task;
		}

		public virtual void Send()
		{
			try
			{
				if (mustReconnect) Connect(false, CancellationToken.None);
				if (IsAlive) PerformSend();
			}
			catch (Exception e)
			{
				if (failurePolicy.ShouldFail())
				{
					IsAlive = false;
					HandleIOFail(e);
					throw;
				}

				mustReconnect = true;
				owner.NeedsIO(this);
			}
		}

		public virtual void Receive()
		{
			try
			{
				if (mustReconnect) Connect(false, CancellationToken.None);
				if (IsAlive) PerformReceive2();
			}
			catch (Exception e)
			{
				if (failurePolicy.ShouldFail())
				{
					IsAlive = false;
					HandleIOFail(e);
					throw;
				}

				mustReconnect = true;
				owner.NeedsIO(this);
			}
		}

		protected virtual void HandleIOFail(Exception e)
		{
			var fail = new IOException("io fail; see inner exception", e);

			FailQueue(bufferQueue, fail);
			FailQueue(readQueue, fail);
			if (currentWriteOp.Task != null) currentWriteOp.Task.SetException(fail);

			currentWriteOp = Data.Empty;
			currentWriteCopier = null;
			inprogressResponse = null;
		}

		private void FailQueue(AdvQueue<Data> queue, Exception e)
		{
			counterErrorPerSec.IncrementBy(queue.Count);

			foreach (var data in queue)
				data.Task.SetException(e);

			queue.Clear();
		}

		/// <summary>
		/// Sends the current chunked op (its data could not fit the write buffer in one pass)
		/// </summary>
		/// <returns>returns true if further IO is required; false if no inprogress op present or the last chunk was successfully added to the buffer</returns>
		private bool SendInProgressOp()
		{
			// check if we have an op in progress
			if (currentWriteCopier == null) return false;
			if (currentWriteCopier.WriteTo(socket.WriteBuffer)) return true;

			// last chunk was sent
			if (LogTraceEnabled) log.Trace("Sent & finished " + currentWriteOp.Op);

			// finished writing, clean up
			bufferQueue.Enqueue(currentWriteOp);
			currentWriteCopier.Dispose();
			currentWriteCopier = null;
			currentWriteOp = Data.Empty;

			return false;
		}

		private void PerformSend()
		{
			Debug.Assert(IsAlive);

			if (sendInProgress) return;

			if (!SendInProgressOp())
			{
				// no in progress op (or just finished), try filling up the buffer
				while (!socket.WriteBuffer.IsFull && DequeueAndWrite()) ;
			}

			// did we write anything?
			if (socket.WriteBuffer.Position > 0)
				FlushWriteBuffer();
		}

		protected virtual void FlushWriteBuffer()
		{
			if (LogTraceEnabled) log.Trace("Flush write buffer " + bufferCounter++);

			counterWritePerSec.Increment();
			sendInProgress = true;

			var sw = Stopwatch.StartNew();

			socket.ScheduleSend(success =>
			{
				if (!success) HandleIOFail(new IOException("send fail"));
				else
				{
					if (bufferQueue.Count > 0) readQueue.Enqueue(bufferQueue);

					gaugeSendSpeed.Set(sw.ElapsedTicks * 1000000 / Stopwatch.Frequency);
					sendInProgress = false;
					owner.NeedsIO(this);
				}
			});
		}

		private uint bufferCounter;

		protected virtual Data GetNextOp()
		{
			Data data;

			if (writeQueue.TryDequeue(out data))
			{
				counterQueue.Decrement();
				return data;
			}

			return Data.Empty;
		}

		private bool DequeueAndWrite()
		{
			var data = GetNextOp();

			if (data.IsEmpty)
				return false;

			counterDequeuePerSec.Increment();
			WriteOp(data);

			return true;
		}

		protected virtual void WriteOp(Data data)
		{
			if (currentWriteCopier != null)
				throw new InvalidOperationException("Cannot write operation while another is in progress.");

			var request = data.Op.CreateRequest();

			if (!request.WriteTo(socket.WriteBuffer)) // fully written
			{
				bufferQueue.Enqueue(data);
				request.Dispose();

				if (LogTraceEnabled) log.Trace("Full send of " + data.Op);
			}
			else
			{
				// it did not fit into the writeBuffer, so save the current op
				// as "in-progress"; PerformSend will loop until it's fully sent
				currentWriteOp = data;
				currentWriteCopier = request;
				if (LogTraceEnabled) log.Trace("Partial send of " + data.Op);
			}
		}

		protected abstract IResponse CreateResponse();

		private bool receiveInProgress;
		private bool sendInProgress;

		private void PerformReceive2()
		{
			Debug.Assert(IsAlive);
			if (readQueue.Count == 0 || sendInProgress || receiveInProgress) return;

		fill:
			// no data to process => read the socket
			if (socket.ReadBuffer.IsEmpty)
			{
				receiveInProgress = true;
				socket.ScheduleReceive(success =>
				{
					if (!success) HandleIOFail(new IOException("receive fail"));
					else
					{
						receiveInProgress = false;
						owner.NeedsIO(this);
					}
				});

				return;
			}

			while (readQueue.Count > 0)
			{
				var response = inprogressResponse ?? CreateResponse();

				if (response.Read(socket.ReadBuffer)) // is IO pending? (if response is not read fully)
				{
					// the ony reason to need data should be an empty receive buffer
					Debug.Assert(socket.ReadBuffer.IsEmpty);
					Debug.Assert(inprogressResponse == null);

					inprogressResponse = response;
					goto fill;
					//return true;
				}

				if (inprogressResponse != null)
				{
					inprogressResponse.Dispose();
					inprogressResponse = null;
				}

				var matching = false;

				while (!matching && readQueue.Count > 0)
				{
					var data = readQueue.Peek();
					matching = data.Op.Handles(response);

					// null is a response to a successful quiet op
					// we have to feed the responses to the current op
					// until it returns false
					if (!data.Op.ProcessResponse(matching ? response : null))
					{
						readQueue.Dequeue();

						if (data.Task != null)
							data.Task.TrySetResult(data.Op);
					}
				}

				response.Dispose();
			}
		}

		protected struct Data
		{
			public static readonly Data Empty = new Data();

			public IOperation Op;
			public TaskCompletionSource<IOperation> Task;

			public bool IsEmpty { get { return Op == null; } }
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
