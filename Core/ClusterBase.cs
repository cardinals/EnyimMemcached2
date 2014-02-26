using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Enyim.Caching
{
	public abstract class ClusterBase : ICluster
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private static readonly bool LogTraceEnabled = log.IsTraceEnabled;
		private static readonly bool LogDebugEnabled = log.IsDebugEnabled;
		private static readonly bool LogInfoEnabled = log.IsInfoEnabled;

		private static readonly TaskCompletionSource<IOperation> failSingle;
		private static readonly TaskCompletionSource<IOperation[]> failBroadcast;

		private readonly IPEndPoint[] endpoints;
		private readonly INodeLocator locator;
		private readonly IReconnectPolicy reconnectPolicy;

		private readonly CancellationTokenSource shutdownToken;

		private readonly Thread worker;
		private readonly ManualResetEventSlim workerIsDone;

		private INode[] allNodes;
		private INode[] workingNodes;
		private NodeQueue ioQueue;

		protected ClusterBase(IEnumerable<IPEndPoint> endpoints, INodeLocator locator, IReconnectPolicy reconnectPolicy)
		{
			this.endpoints = endpoints.ToArray();
			this.locator = locator;
			this.reconnectPolicy = reconnectPolicy;

			this.worker = new Thread(Worker);

			this.shutdownToken = new CancellationTokenSource();
			this.workerIsDone = new ManualResetEventSlim(false);
		}

		static ClusterBase()
		{
			var allDead = new IOException("All nodes are dead.");

			failSingle = new TaskCompletionSource<IOperation>();
			failBroadcast = new TaskCompletionSource<IOperation[]>();
			failSingle.SetException(allDead);
			failBroadcast.SetException(allDead);
		}

		protected abstract INode CreateNode(IPEndPoint endpoint);

		public virtual void Start()
		{
			allNodes = endpoints.Select(CreateNode).ToArray();
			ioQueue = new NodeQueue(allNodes);
			workingNodes = allNodes.ToArray();
			locator.Initialize(allNodes);

			worker.Start();
		}

		public virtual void Dispose()
		{
			if (!shutdownToken.IsCancellationRequested)
			{
				shutdownToken.Cancel();
				workerIsDone.Wait();

				foreach (var node in allNodes)
				{
					try { node.Shutdown(); }
					catch (Exception e)
					{
						if (log.IsErrorEnabled)
							log.Error("Error while shutting down " + node, e);
					}
				}
			}

			SocketAsyncEventArgsPool.Instance.Compact();
		}

		public virtual Task<IOperation> Execute(IItemOperation op)
		{
			var node = locator.Locate(op.Key);

			if (!node.IsAlive)
				return failSingle.Task;

			var retval = node.Enqueue(op);
			ioQueue.Add(node);

			return retval;
		}

		public virtual Task<IOperation[]> Broadcast(Func<INode, IOperation> createOp)
		{
			// create local "copy" of the reference, as
			// workingNodes is never changed but replaced
			var nodes = workingNodes;
			var tasks = new List<Task<IOperation>>(nodes.Length);

			foreach (var node in nodes)
			{
				if (node.IsAlive)
				{
					tasks.Add(node.Enqueue(createOp(node)));
					ioQueue.Add(node);
				}
			}

			if (tasks.Count == 0)
				return failBroadcast.Task;

			return Task.WhenAll(tasks);
		}

		private void Worker()
		{
			while (!shutdownToken.IsCancellationRequested)
			{
				try
				{
					var node = ioQueue.Take(shutdownToken.Token);

					try
					{
						node.Send();
						if (shutdownToken.IsCancellationRequested) break;
						node.Receive();
					}
					catch (Exception e)
					{
						FailNode(node, e);
					}
				}
				catch (OperationCanceledException)
				{
					break;
				}
			}

			if (LogDebugEnabled) log.Debug("shutdownToken was cancelled, finishing work");

			workerIsDone.Set();
		}

		protected virtual void ScheduleReconnect(INode node)
		{
			if (LogInfoEnabled) log.Info("Scheduling reconnect for " + node.EndPoint);

			var when = reconnectPolicy.Schedule(node);

			if (when == TimeSpan.Zero)
			{
				if (LogInfoEnabled) log.Info("Will reconnect now");
				ReconnectNow(node, false);
			}
			else
			{
				if (LogInfoEnabled) log.Info("Will reconnect after " + when);
				Task
					.Delay(when, shutdownToken.Token)
					.ContinueWith(_ => ReconnectNow(node, true),
									TaskContinuationOptions.OnlyOnRanToCompletion);
			}
		}

		protected virtual void ReconnectNow(INode node, bool reset)
		{
			try
			{
				if (shutdownToken.IsCancellationRequested) return;

				node.Connect(reset, shutdownToken.Token);

				ReAddNode(node);
				ioQueue.Add(node); // trigger IO on this node
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled)
					log.Error("Failed to reconnect", e);

				ScheduleReconnect(node);
			}
		}

		private void ReAddNode(INode node)
		{
			if (LogDebugEnabled) log.Debug("Node {0} was reconnected", node.EndPoint);

			reconnectPolicy.Reset(node);

			var existing = Volatile.Read(ref workingNodes);

			while (true)
			{
				var updated = new INode[existing.Length + 1];
				Array.Copy(existing, 0, updated, 0, existing.Length);
				updated[existing.Length] = node;

				var current = Interlocked.CompareExchange(ref workingNodes, updated, existing);

				if (Object.ReferenceEquals(existing, current))
				{
					locator.Initialize(updated);
					break;
				}

				existing = current;
			}

			locator.Initialize(existing);
		}

		private void FailNode(INode node, Exception e)
		{
			if (log.IsWarnEnabled) log.Warn("Node {0} failed", node.EndPoint);

			var existing = Volatile.Read(ref workingNodes);

			while (true)
			{
				var updated = existing.Where(n => n != node).ToArray();
				var current = Interlocked.CompareExchange(ref workingNodes, updated, existing);

				if (Object.ReferenceEquals(existing, current))
				{
					locator.Initialize(updated);
					break;
				}

				existing = current;
			}

			ScheduleReconnect(node);
		}

		public void NeedsIO(INode node)
		{
			if (LogTraceEnabled) log.Trace("Node {0} has pending IO, requeueing", node);
			ioQueue.Add(node);
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
