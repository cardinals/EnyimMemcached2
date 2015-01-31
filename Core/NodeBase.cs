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
		private static readonly bool LogInfoEnabled = log.IsInfoEnabled;

		private readonly ICluster owner;
		private readonly IPEndPoint endpoint;
		private readonly IFailurePolicy failurePolicy;

		private readonly ISocket socket;

		private object failLock;
		private readonly ConcurrentQueue<Data> writeQueue;
		private readonly AdvQueue<Data> readQueue;

		private int state;
		private IRequest currentWriteCopier;
		private Data currentWriteOp;
		private IResponse inprogressResponse;

		private bool mustReconnect;

		private readonly ICounter counterEnqueuePerSec;
		private readonly ICounter counterDequeuePerSec;
		private readonly ICounter counterOpReadPerSec;
		private readonly ICounter counterWritePerSec;
		private readonly ICounter counterErrorPerSec;
		private readonly ICounter counterItemCount;
		private readonly ICounter counterWriteQueue;
		private readonly ICounter counterReadQueue;
		private readonly IGauge gaugeSendSpeed;

		protected NodeBase(IPEndPoint endpoint, ICluster owner, IFailurePolicy failurePolicy, Func<ISocket> socket)
		{
			counterEnqueuePerSec = Metrics.Meter("node write enqueue/sec", endpoint.ToString(), Interval.Seconds);
			counterDequeuePerSec = Metrics.Meter("node write dequeue/sec", endpoint.ToString(), Interval.Seconds);
			counterOpReadPerSec = Metrics.Meter("node op read/sec", endpoint.ToString(), Interval.Seconds);
			counterWriteQueue = Metrics.Counter("write queue length", endpoint.ToString());
			counterReadQueue = Metrics.Counter("read queue length", endpoint.ToString());

			counterWritePerSec = Metrics.Meter("node write/sec", endpoint.ToString(), Interval.Seconds);
			counterErrorPerSec = Metrics.Meter("node in error/sec", endpoint.ToString(), Interval.Seconds);
			counterItemCount = Metrics.Counter("commands", endpoint.ToString());
			gaugeSendSpeed = Metrics.Gauge("send speed", endpoint.ToString());

			this.owner = owner;
			this.endpoint = endpoint;
			this.socket = socket();
			this.failurePolicy = failurePolicy;

			this.failLock = new Object();
			this.writeQueue = new ConcurrentQueue<Data>();
			this.readQueue = new AdvQueue<Data>();

			this.mustReconnect = true;
			IsAlive = true;
		}

		protected abstract IResponse CreateResponse();

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
				counterWriteQueue.Increment();
			}
			else
			{
				counterErrorPerSec.Increment();
				tcs.SetException(new IOException(endpoint + " is not alive"));
			}

			return tcs.Task;
		}

		private int __working;

		private bool Lock()
		{
			return Interlocked.CompareExchange(ref __working, 1, 0) == 0;
		}

		private void Unlock()
		{
#if DEBUG
			var v = Interlocked.CompareExchange(ref __working, 0, 1);
			Debug.Assert(v == 1);
#else
			Interlocked.Exchange(ref __working, 0);
#endif
		}

		public virtual void Run()
		{
			if (!Lock()) return;

			try
			{
				if (mustReconnect) Connect(false, CancellationToken.None);
				if (!IsAlive) throw new IOException("Node is dead: " + EndPoint);

				// socket still working but we got requeued, so quit
				// happens when OPs are coming in, but we're still processing the previous batch
				if (socket.IsWorking)
				{
					if (LogTraceEnabled) log.Trace("Node {0}'s socket is working", endpoint);
					return;
				}

				DoRun();
			}
			catch (Exception e)
			{
				if (FailMe(e)) throw;
			}
		}

		private const int SEND = 0;
		private const int RECEIVE = 1;
		private int runMode;

		private void DoRun()
		{
			switch (runMode)
			{
				case SEND:
					// write the current (in progress) op into the write buffer
					// - or -
					// start writing a new one (until we run out of ops or space)
					if (!ContinueWritingCurrentOp())
					{
						while (!socket.WriteBuffer.IsFull)
						{
							var data = GetNextOp();
							if (data.IsEmpty) break;

							counterDequeuePerSec.Increment();
							WriteOp(data);
						}
					}

					// did we write anything?
					if (socket.WriteBuffer.Position > 0)
					{
						FlushWriteBuffer();
					}
					else
					{
						// did not have any ops to send, quit
						Unlock();
					}

					break;

				case RECEIVE:
					// do we have ops queued to be read?
					if (readQueue.Count > 0)
					{
						PerformReceive();
					}
					else
					{
						runMode = SEND;
						goto case SEND;
					}
					break;
			}
		}

		/// <summary>
		/// Sends the current chunked op. Happens when an ops data cannot fit the write buffer in one pass.
		/// </summary>
		/// <returns>returns true if further IO is required; false if no inprogress op present or the last chunk was successfully added to the buffer</returns>
		private bool ContinueWritingCurrentOp()
		{
			// check if we have an op in progress
			if (currentWriteCopier == null) return false;
			if (currentWriteCopier.WriteTo(socket.WriteBuffer)) return true;

			// last chunk was sent
			if (LogTraceEnabled) log.Trace("Sent & finished " + currentWriteOp.Op);


			// op is sent fully; response can be expected
			readQueue.Enqueue(currentWriteOp);
			counterReadQueue.Increment();

			// clean up
			currentWriteCopier.Dispose();
			currentWriteCopier = null;
			currentWriteOp = Data.Empty;

			return false;
		}

		protected virtual void FlushWriteBuffer()
		{
			counterWritePerSec.Increment();

			socket.ScheduleSend(success =>
			{
				if (success)
				{
					Volatile.Write(ref runMode, RECEIVE);
					Unlock();
					owner.NeedsIO(this);
				}
				else
				{
					// this is a soft fail (cannot throw from other thread)
					// so we requeue for IO and Run() will throw instead
					FailMe(new IOException("send fail"));
				}
			});
		}

		protected virtual Data GetNextOp()
		{
			Data data;

			if (writeQueue.TryDequeue(out data))
			{
				counterWriteQueue.Decrement();
				return data;
			}

			return Data.Empty;
		}

		/// <summary>
		/// Writes an operation to the output buffer. Handles the case where the op does not fit the buffer fully.
		/// </summary>
		/// <param name="data"></param>
		protected virtual void WriteOp(Data data)
		{
			if (currentWriteCopier != null)
				throw new InvalidOperationException("Cannot write operation while another is in progress.");

			var request = data.Op.CreateRequest();

			if (!request.WriteTo(socket.WriteBuffer)) // fully written
			{
				readQueue.Enqueue(data);
				counterReadQueue.Increment();
				request.Dispose();

				if (LogTraceEnabled) log.Trace("Full send of " + data.Op);
			}
			else
			{
				// it did not fit into the write buffer, so save the current op
				// as "in-progress"; DoRun will loop until it's fully sent
				currentWriteOp = data;
				currentWriteCopier = request;
				if (LogTraceEnabled) log.Trace("Partial send of " + data.Op);
			}
		}

		private void PerformReceive()
		{
		fill:
			// no data to process => read the socket
			if (socket.ReadBuffer.IsEmpty)
			{
				if (LogTraceEnabled) log.Trace("Read buffer is empty, ask for more.");

				socket.ScheduleReceive(success =>
				{
					if (success)
					{
						Unlock();
						owner.NeedsIO(this);
					}
					else
					{
						// this is a soft fail (cannot throw from other thread)
						// so we requeue for IO and exception will be thrown by Receive()
						FailMe(new IOException("Failed receiving from " + endpoint));
					}
				});

				return;
			}

			// process the commands in the readQueue
			while (readQueue.Count > 0)
			{
				// continue filling the previously unfinished response,
				// or create a new one
				var response = inprogressResponse ?? CreateResponse();

				// continue filling the Response object from the buffer
				// Read() returns true if further data (IO) is required
				// (usually when the current response data is larger than the receive buffer size)
				if (response.Read(socket.ReadBuffer))
				{
					inprogressResponse = response;
					if (LogTraceEnabled) log.Trace("Response is not read fully, continue reading from the socket.");

					// refill the buffer
					// TODO if Receive returns synchrously several times, a node with a huge inprogress response can monopolize the IO thread
					goto fill;
				}

				// successfully read a response from the read buffer
				inprogressResponse = null;
				var matching = false;

				while (!matching && readQueue.Count > 0)
				{
					var data = readQueue.Peek();
					Debug.Assert(!data.IsEmpty);

					// if the response does not matches the current op, it means it's a
					// response to later command in the queue, so all commands before it are silent commands
					// successful silent ops will receive null as response (since we have no real response)
					// (or we've ran into a bug)
					matching = data.Op.Handles(response);
					if (LogTraceEnabled) log.Trace("Command {0} handles reponse: {1}", data.Op, matching);

					// returns false when no more IO is required => command is processed
					// otherwise continue filling the buffer
					if (!data.Op.ProcessResponse(matching ? response : null))
					{
						readQueue.Dequeue();
						counterReadQueue.Decrement();
						counterOpReadPerSec.Increment();

						if (data.Task != null)
							data.Task.TrySetResult(data.Op);
					}
				}

				response.Dispose();
			}

			// set the node into receive mode and requeue for IO
			runMode = SEND;
			Unlock();
			owner.NeedsIO(this);
		}

		protected struct Data
		{
			public static readonly Data Empty = new Data();

			public IOperation Op;
			public TaskCompletionSource<IOperation> Task;

			public bool IsEmpty { get { return Op == null; } }
		}

		#region [ Failure handlers             ]

		private bool FailMe(Exception e)
		{
			lock (failLock)
			{
				var fail = (e is IOException) ? e : new IOException("io fail; see inner exception", e);

				// empty all queues
				FailQueue(writeQueue, fail);
				FailQueue(readQueue, fail);
				counterReadQueue.Reset();
				counterWriteQueue.Reset();

				// kill the partially sent op (if any)
				if (currentWriteCopier != null)
				{
					currentWriteOp.Task.SetException(fail);
					currentWriteOp = Data.Empty;
					currentWriteCopier = null;
				}

				// kill the partially read response
				if (inprogressResponse != null)
				{
					inprogressResponse.Dispose();
					inprogressResponse = null;
				}

				// mark as dead if policy says so
				if (failurePolicy.ShouldFail(this))
				{
					IsAlive = false;
					return true;
				}

				// otherwise reconnect immediately
				// (when it's our turn again, to be precise)
				mustReconnect = true;
				runMode = SEND;
				// reconnect from IO thread
				Unlock();
				owner.NeedsIO(this);

				return false;
			}
		}

		/// <summary>
		/// Cleans up an AdvQueue, marking all items as failed
		/// </summary>
		private void FailQueue(AdvQueue<Data> queue, Exception e)
		{
			foreach (var data in queue)
			{
				var t = data.Task;
				if (t != null) t.TrySetException(e);
				counterErrorPerSec.IncrementBy(queue.Count);
			}

			queue.Clear();
		}

		/// <summary>
		/// Cleans up a ConcurrentQueue, marking all items as failed
		/// </summary>
		private void FailQueue(ConcurrentQueue<Data> queue, Exception e)
		{
			Data data;
			var have = queue.Count;
			var i = 0;

			while (queue.TryDequeue(out data) && i < have)
			{
				var t = data.Task;
				if (t != null) t.SetException(e);
				counterErrorPerSec.IncrementBy(have);
				i++;
			}
		}

		#endregion
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
