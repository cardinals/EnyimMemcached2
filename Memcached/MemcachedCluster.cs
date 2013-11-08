using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class MemcachedCluster : ICluster
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private static readonly TaskCompletionSource<bool> fail;

		private readonly Func<IPEndPoint, INode> nodeFactory;
		private readonly INodeLocator locator;
		private readonly IReconnectPolicy policy;

		private readonly CancellationTokenSource shutdownToken;

		private Thread worker;
		private ManualResetEventSlim workerQuit;
		private ManualResetEventSlim hasWork;
		private INode[] allNodes;
		private INode[] workingNodes;
		private ConcurrentQueue<INode> reconnectedNodes;

		public MemcachedCluster(IEnumerable<IPEndPoint> endpoints,
								INodeLocator locator, 
								IReconnectPolicy policy, 
								Func<IPEndPoint, INode> nodeFactory)
		{
			this.policy = policy;
			this.locator = locator;
			this.nodeFactory = nodeFactory;

			this.shutdownToken = new CancellationTokenSource();
			this.hasWork = new ManualResetEventSlim();
			this.worker = new Thread(Worker) { Name = "The Worker" };
			this.reconnectedNodes = new ConcurrentQueue<INode>();
			this.allNodes = endpoints.Select(nodeFactory).ToArray();
		}

		static MemcachedCluster()
		{
			fail = new TaskCompletionSource<bool>();
			fail.SetException(new IOException());
		}

		public virtual void Start()
		{
			workingNodes = allNodes.ToArray();

			workerQuit = new ManualResetEventSlim(false);

			Parallel.ForEach(allNodes, n =>
			{
				try { n.Connect(true, shutdownToken.Token); }
				catch (Exception e) { FailNode(n, e); }
			});

			locator.Initialize(workingNodes);

			worker.Start();
		}

		public virtual void Dispose()
		{
			shutdownToken.Cancel();
			workerQuit.Wait();

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

		public virtual Task Execute(ISingleItemOperation op)
		{
			var node = locator.Locate(op.Key);

			if (node.IsAlive)
			{
				var retval = node.Enqueue(op);
				hasWork.Set();

				return retval;
			}

			return fail.Task;
		}

		public virtual Task Broadcast(Func<IOperation> op)
		{
			var nodes = workingNodes; // create local copy
			var tasks = new List<Task>(nodes.Length);

			foreach (var node in nodes)
			{
				if (node.IsAlive)
					tasks.Add(node.Enqueue(op()));
			}

			if (tasks.Count > 0)
			{
				hasWork.Set();

				return tasks.Count == 1 ? tasks[0] : Task.WhenAll(tasks);
			}

			return fail.Task;
		}

		private void Worker()
		{
			while (!shutdownToken.IsCancellationRequested)
			{
				hasWork.Reset();

				var pendingIO = false;

				if (RunOnNodes(n => n.Send())) pendingIO = true;
				if (shutdownToken.IsCancellationRequested) break;
				if (RunOnNodes(n => n.Receive())) pendingIO = true;

				if (!pendingIO)
				{
					if (log.IsTraceEnabled) log.Trace("No pending IO, waiting");

					try { hasWork.Wait(shutdownToken.Token); }
					catch (OperationCanceledException) { break; }
				}
			}

			if (log.IsDebugEnabled) log.Debug("shutdownToken was cancelled, finishing work");

			workerQuit.Set();
		}

		protected virtual bool RunOnNodes(Func<INode, bool> what)
		{
			var pendingIO = false;
			var runOn = workingNodes;

			foreach (var node in runOn)
			{
				try
				{
					if (what(node))
					{
						if (log.IsTraceEnabled) log.Trace(node + " has pending IO.");
						pendingIO = true;
					}
				}
				catch (Exception e)
				{
					FailNode(node, e);
				}
			}

			return pendingIO;
		}

		protected virtual void ScheduleReconnect(INode node)
		{
			if (log.IsInfoEnabled) log.Info("Scheduling reconnect for " + node.EndPoint);

			var when = policy.Schedule(node);
			if (log.IsDebugEnabled) log.Debug("Will reconnect after " + when);

			if (when == TimeSpan.Zero)
			{
				ReconnectNow(node, false);
			}
			else
			{
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
			if (log.IsDebugEnabled) log.Debug("Node {0} was reconnected", node.EndPoint);

			policy.Reset(node);

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
			hasWork.Set();
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
