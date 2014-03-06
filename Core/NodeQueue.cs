using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Enyim.Caching
{
	internal class NodeQueue
	{
		private readonly BlockingCollection<INode> queue;
		private readonly NonLockingIndexSet set;
		private readonly Dictionary<INode, int> nodeIndexes;

		internal NodeQueue(INode[] allNodes)
		{
			queue = new BlockingCollection<INode>();
			set = new NonLockingIndexSet(allNodes.Length);
			nodeIndexes = Enumerable
									.Range(0, allNodes.Length)
									.ToDictionary(k => allNodes[k], k => k);
		}

		public void Add(INode node)
		{
			if (set.Set(nodeIndexes[node]))
				queue.Add(node);
		}

		public INode Take(CancellationToken token)
		{
			var retval = queue.Take(token);
			set.Unset(nodeIndexes[retval]);

			return retval;
		}

		#region [ ConcurrentIndexSet           ]

		private class ConcurrentIndexSet
		{
			private const int TRUE = 1;
			private const int FALSE = 0;

			private readonly int[] data;

			public ConcurrentIndexSet(int capacity)
			{
				data = new int[capacity];
			}

			public bool Set(int index)
			{
				return Interlocked.CompareExchange(ref data[index], TRUE, FALSE) == FALSE;
			}

			public bool Unset(int index)
			{
				return Interlocked.CompareExchange(ref data[index], FALSE, TRUE) == TRUE;
			}

			public bool Contains(int index)
			{
				return Interlocked.CompareExchange(ref data[index], -1, -1) == TRUE;
			}
		}

		#endregion
		#region [ NonLockingIndexSet           ]
		// TODO check if we need memory barriers
		private class NonLockingIndexSet
		{
			private const int TRUE = 1;
			private const int FALSE = 0;
			private const int GAP = 64 / sizeof(int);

			private readonly int[] data;

			public NonLockingIndexSet(int capacity)
			{
				data = new int[capacity * GAP];
			}

			public bool Set(int index)
			{
				var old = data[index * GAP];
				data[index * GAP] = TRUE;

				return old == FALSE;
			}

			public bool Unset(int index)
			{
				var old = data[index * GAP];
				data[index * GAP] = FALSE;

				return old == TRUE;
			}

			public bool Contains(int index)
			{
				return data[index * GAP] == TRUE;
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
