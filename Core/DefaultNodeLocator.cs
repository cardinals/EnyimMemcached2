﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class DefaultNodeLocator : INodeLocator
	{
		private readonly object InitLock = new Object();
		private InnerLocator locator;

		public void Initialize(IEnumerable<INode> currentNodes)
		{
			lock (InitLock)
			{
				var tmp = new InnerLocator(currentNodes);
				Interlocked.Exchange(ref locator, tmp);
			}
		}

		public INode Locate(Key key) => locator.Locate(key);

		#region [ InnerLocator                 ]

		private class InnerLocator
		{
			private readonly INode[] nodes;
			private readonly int bucketCount;

			public InnerLocator(IEnumerable<INode> currentNodes)
			{
				nodes = currentNodes.ToArray();
				bucketCount = nodes.Length;
			}

			public INode Locate(Key key)
			{
				if (bucketCount == 0) return AlreadyFailedNode.Instance;
				if (bucketCount == 1) return nodes[0];

				var hash = Murmur64_64.ComputeHash(key.Array, key.Length, 0);
				var bucketIndex = JumpConsistentHash(hash, bucketCount);

				return nodes[bucketIndex];
			}

			private static int JumpConsistentHash(ulong key, int bucketCount)
			{
				Debug.Assert(bucketCount > 0);

				const ulong MULTIPLIER = 2862933555777941757;

				ulong retval = 0;
				ulong index = 0;
				ulong ulongCount = (ulong)bucketCount;

				while (index < ulongCount)
				{
					retval = index;
					key = key * MULTIPLIER + 1;
					index = (ulong)((retval + 1) * (double)(1L << 31) / ((key >> 33) + 1));
				}

				return (int)retval;
			}
		}

		#endregion
		#region [ AlreadyFailedNode            ]

		private class AlreadyFailedNode : INode
		{
			private static readonly Lazy<TaskCompletionSource<IOperation>> FailedTask = new Lazy<TaskCompletionSource<IOperation>>(() =>
			{
				var retval = new TaskCompletionSource<IOperation>();
				retval.SetException(new IOException("AlwaysDead"));

				return retval;
			});

			public static readonly INode Instance = new AlreadyFailedNode();

			public bool IsAlive { get; } = false;
			public IPEndPoint EndPoint { get; } = new IPEndPoint(IPAddress.None, 0);

			public void Run() { }
			public void Connect(CancellationToken token) { }
			public void Shutdown() { }

			public Task<IOperation> Enqueue(IOperation op) => FailedTask.Value.Task;
			public override string ToString() => "AlreadyFailedNode";
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
