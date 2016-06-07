using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.Tracing;

namespace Enyim.Caching
{
	public abstract class NodeBase : INode
	{
		private readonly ICluster owner;
		private readonly IPEndPoint endpoint;
		private readonly IFailurePolicy failurePolicy;
		private readonly string name; // used for tracing

		private ISocket socket;

		private bool isAlive; // if the socket is alive (ALIVE) or not (DEAD)
		private int currentlyReading; // if a read is in progress, to prevent re-entrancy on reads when a node is queued for IO multiple times
		private int currentlyWriting; // if a write is in progress, to prevent re-entrancy on writes when a node is queued for IO multiple times

		private object failLock;

		private readonly ConcurrentQueue<OpQueueEntry> writeQueue;
		private readonly Queue<OpQueueEntry> readQueue;

		private OpQueueEntry currentWriteOp;
		private IRequest currentWriteCopier;
		private IResponse inprogressResponse;

		private bool mustReconnect;
		private NodePerformanceMonitor npm;

		protected NodeBase(ICluster owner, IPEndPoint endpoint, IFailurePolicy failurePolicy, ISocket socket)
		{
			this.owner = owner;
			this.endpoint = endpoint;
			this.socket = socket;
			this.failurePolicy = failurePolicy;

			name = endpoint.ToString();
			failLock = new Object();
			writeQueue = new ConcurrentQueue<OpQueueEntry>();
			readQueue = new Queue<OpQueueEntry>();

			mustReconnect = true;
			IsAlive = true;

			npm = new NodePerformanceMonitor(name);
		}

		protected abstract IResponse CreateResponse();

		protected WriteBuffer WriteBuffer { get { return socket.WriteBuffer; } }
		public IPEndPoint EndPoint { get { return endpoint; } }

		public bool IsAlive
		{
			get { return Volatile.Read(ref isAlive); }
			private set { Volatile.Write(ref isAlive, value); }
		}

		public void Connect()
		{
			Connect(CancellationToken.None);
		}

		public virtual void Connect(CancellationToken token)
		{
			Debug.Assert(currentWriteCopier == null);
			Debug.Assert(inprogressResponse == null);
			Debug.Assert(readQueue.Count == 0);

			LogTo.Info($"Connecting node to {name}");

			socket.Connect(endpoint, token);

			mustReconnect = false;
			IsAlive = true;
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

		public virtual Task<IOperation> Enqueue(IOperation op)
		{
			var tcs = new TaskCompletionSource<IOperation>();
			npm.NewOp();

			try
			{
				if (IsAlive)
				{
					writeQueue.Enqueue(new OpQueueEntry(op, tcs));

					#region Diagnostics
					npm.EnqueueWriteOp();
					CoreEventSource.EnqueueWriteOp(name);
					#endregion
				}
				else
				{
					tcs.TrySetException(new IOException(endpoint + " is not alive"));

					#region Diagnostics
					npm.Error();
					CoreEventSource.NodeError(name);
					#endregion
				}
			}
			catch (Exception e)
			{
				tcs.TrySetException(new IOException(endpoint + " enqueue failed. See inner excption for details.", e));

				#region Diagnostics
				npm.Error();
				CoreEventSource.NodeError(name);
				#endregion
			}

			return tcs.Task;
		}

		public virtual void Run()
		{
			try
			{
				if (mustReconnect) Connect(CancellationToken.None);
				if (!IsAlive)
				{
					var dead = new IOException($"Node is dead: {name}");
					FailQueue(writeQueue, dead);
					throw dead;
				}

				DoRun();
			}
			catch (Exception e)
			{
				if (FailMe(e)) throw;
			}
		}

		private void DoRun()
		{
			if (_TryStartWriting())
			{
				// write the current (in progress) op into the write buffer
				// - or -
				// start writing a new one (until we run out of ops or space)
				if (!ContinueWritingCurrentOp())
				{
					while (!socket.WriteBuffer.IsFull)
					{
						var data = GetNextOp();
						if (data.IsEmpty) break;

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
					_FinishedWriting();
				}
			}

			if (_TryStartReading())
			{
				TryAskForMoreData();

				if (!socket.IsReceiving)
					TryProcessReceivedData();
			}
		}

		/// <summary>
		/// Sends the current chunked op. Happens when an op's data cannot fit the write buffer in one pass.
		/// </summary>
		/// <returns>returns true if further IO is required; false if no inprogress op present or the last chunk was successfully added to the buffer</returns>
		private bool ContinueWritingCurrentOp()
		{
			// check if we have an op in progress
			if (currentWriteCopier == null) return false;
			if (currentWriteCopier.WriteTo(socket.WriteBuffer)) return true;

			// last chunk was sent
			LogTo.Trace($"Sent & finished {currentWriteOp.Op}");

			// op is sent fully; response can be expected
			readQueue.Enqueue(currentWriteOp);

			npm.EnqueueReadOp();
			CoreEventSource.EnqueueReadOp(name);

			// clean up
			currentWriteCopier.Dispose();
			currentWriteCopier = null;
			currentWriteOp = OpQueueEntry.Empty;

			return false;
		}

		protected virtual void FlushWriteBuffer()
		{
			npm.Flush();

			socket.ScheduleSend(success =>
			{
				if (success)
				{
					LogTo.Trace($"{name} send the write buffer successfully");

					_FinishedWriting();
					owner.NeedsIO(this);

					return;
				}

				// this is a soft fail (cannot throw from other thread)
				// so we requeue for IO and Run() will throw instead
				LogTo.Trace($"{name}'s FlushWriteBuffer failed");
				FailMe(new IOException("send fail"));
			});
		}

		protected virtual OpQueueEntry GetNextOp()
		{
			OpQueueEntry data;

			if (writeQueue.TryDequeue(out data))
			{
				npm.DequeueWriteOp();
				CoreEventSource.DequeueWriteOp(name);

				return data;
			}

			return OpQueueEntry.Empty;
		}

		/// <summary>
		/// Writes an operation to the output buffer. Handles the case where the op does not fit the buffer fully.
		/// </summary>
		/// <param name="data"></param>
		protected virtual void WriteOp(OpQueueEntry data)
		{
			if (currentWriteCopier != null)
				throw new InvalidOperationException("Cannot write an operation while another is in progress.");

			var request = data.Op.CreateRequest();

			if (!request.WriteTo(socket.WriteBuffer)) // no pending IO => fully written
			{
				readQueue.Enqueue(data);
				request.Dispose();

				npm.EnqueueReadOp();
				CoreEventSource.EnqueueReadOp(name);

				LogTo.Trace($"Full send of {data.Op}");
			}
			else
			{
				// it did not fit into the write buffer, so save the current op
				// as "in-progress"; DoRun will loop until it's fully sent
				currentWriteOp = data;
				currentWriteCopier = request;
				LogTo.Trace($"Partial send of {data.Op}");
			}
		}

		private void TryAskForMoreData()
		{
			// no data to process => read the socket
			if (socket.ReadBuffer.IsEmpty && !socket.IsReceiving)
			{
				LogTo.Trace("Read buffer is empty, ask for more.");

				socket.ScheduleReceive(success =>
				{
					if (success)
					{
						LogTo.Trace($"{name} successfully received data");
						_FinishedReading();
						owner.NeedsIO(this);

						return;
					}

					// this is a soft fail (cannot throw from other thread),
					// so we requeue for IO and exception will be thrown by Receive()
					FailMe(new IOException("Failed receiving from " + endpoint));
				});
			}
		}

		private void TryProcessReceivedData()
		{
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
					LogTo.Trace("Response is not read fully, continue reading from the socket.");

					// refill the buffer
					_FinishedReading();
					owner.NeedsIO(this);

					return;
				}

				// successfully read a response from the read buffer
				inprogressResponse = null;
				var isHandled = false;

				while (!isHandled && readQueue.Count > 0)
				{
					var data = readQueue.Peek();
					Debug.Assert(!data.IsEmpty);

					// If the response does not matches the current op, it means it's a
					// response to a later command in the queue, so all commands before it
					// were silent commands without a response (== usually success).
					// So, successful silent ops will receive null as response (since
					// we have no real response (or we've ran into a bug))
					isHandled = data.Op.Handles(response);
					LogTo.Trace($"Command {data.Op} handles reponse: {isHandled}");

					// returns false when no more IO is required => command is processed
					// otherwise continue filling the buffer
					if (!data.Op.ProcessResponse(isHandled ? response : null))
					{
						readQueue.Dequeue();
						//if (data.Task != null)
						data.Task.TrySetResult(data.Op);

						#region Diagnostics
						npm.DequeueReadOp();
						CoreEventSource.DequeueReadOp(name);
						#endregion
					}
				}

				response.Dispose();
			}

			LogTo.Trace($"{name} fininshed RECEIVE, unlock read");
			_FinishedReading();
		}

		public override string ToString()
		{
			return name;
		}

		#region [ Busy signals                 ]

		private bool _TryStartReading()
		{
			return Interlocked.CompareExchange(ref currentlyReading, 1, 0) == 0;
		}

		private bool _TryStartWriting()
		{
			return Interlocked.CompareExchange(ref currentlyWriting, 1, 0) == 0;
		}

		private void _FinishedReading()
		{
			Volatile.Write(ref currentlyReading, 0);
		}

		private void _FinishedWriting()
		{
			Volatile.Write(ref currentlyWriting, 0);
		}

		#endregion
		#region [ OpQueueEntry                 ]

		protected struct OpQueueEntry
		{
			public static readonly OpQueueEntry Empty = new OpQueueEntry();

			public OpQueueEntry(IOperation op, TaskCompletionSource<IOperation> task)
			{
				Debug.Assert(op != null, "OpQueueEntry.Op cannot be null");
				Debug.Assert(task != null, "OpQueueEntry.Task cannot be null");

				Op = op;
				Task = task;
			}

			public readonly IOperation Op;
			public readonly TaskCompletionSource<IOperation> Task;

			public bool IsEmpty { get { return Op == null; } }
		}

		#endregion
		#region [ Failure handlers             ]

		private bool FailMe(Exception e)
		{
			lock (failLock)
			{
				LogTo.Error(e, $"Node {name} has failed during IO.");
				var fail = (e is IOException) ? e : new IOException("io fail; see inner exception", e);

				// empty all queues
				FailQueue(writeQueue, fail);
				FailQueue(readQueue, fail);

				npm.ResetQueues();

				// kill the partially sent op (if any)
				if (currentWriteCopier != null)
				{
					currentWriteOp.Task.SetException(fail);
					currentWriteOp = OpQueueEntry.Empty;
					currentWriteCopier = null;
				}

				// kill the partially read response
				if (inprogressResponse != null)
				{
					inprogressResponse.Dispose();
					inprogressResponse = null;
				}

				_FinishedReading();
				_FinishedWriting();

				// mark as dead if policy says so...
				if (failurePolicy.ShouldFail(this))
				{
					IsAlive = false;
					return true;
				}

				// ...otherwise reconnect immediately (when it's our turn)
				mustReconnect = true;
				LogTo.Info($"Node {endpoint} will reconnect immediately.");

				// reconnect from IO thread
				owner.NeedsIO(this);

				return false;
			}
		}

		/// <summary>
		/// Cleans up an AdvQueue, marking all items as failed
		/// </summary>
		private void FailQueue(Queue<OpQueueEntry> queue, Exception e)
		{
			foreach (var data in queue)
			{
				var t = data.Task;
				if (t == null) break;
				t.TrySetException(e);

				npm.Error();
				CoreEventSource.NodeError(name);
			}

			queue.Clear();
		}

		/// <summary>
		/// Cleans up a ConcurrentQueue, marking all items as failed
		/// </summary>
		private void FailQueue(ConcurrentQueue<OpQueueEntry> queue, Exception e)
		{
			OpQueueEntry data;

			while (queue.TryDequeue(out data))
			{
				var t = data.Task;
				if (t != null) t.SetException(e);

				npm.Error();
				CoreEventSource.NodeError(name);
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
