using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class DefaultNodeLocator : INodeLocator
	{
		private const int ServerAddressMutations = 160;
		private static readonly Encoding NoPreambleUtf8 = new UTF8Encoding(false);

		private INode[] nodes;
		private uint[] keyRing;
		private Dictionary<uint, INode> keyToServer;

		public void Initialize(IEnumerable<INode> currentNodes)
		{
			// quit if we've been initialized because we can handle dead nodes,
			// so there is no need to recalculate everything
			if (keyRing != null) return;

			nodes = currentNodes.ToArray();
			keyRing = new uint[this.nodes.Length * ServerAddressMutations];
			keyToServer = new Dictionary<uint, INode>(keyRing.Length);

			var i = 0;

			foreach (var node in nodes)
			{
				for (var mutation = 0; mutation < ServerAddressMutations; mutation++)
				{
					var address = node.EndPoint.ToString();
					var hash = Murmur32.ComputeHash(Encoding.ASCII.GetBytes(address + "-" + mutation));

					keyRing[i++] = hash;
					keyToServer[hash] = node;
				}
 			}

			Array.Sort<uint>(keyRing);
		}

		private static uint GetKeyHash(string key)
		{
			return Murmur32.ComputeHash(NoPreambleUtf8.GetBytes(key));
		}

		public INode Locate(string key)
		{
			if (key == null) throw new ArgumentNullException("key");

			switch (nodes.Length)
			{
				case 0: return O.Instance;
				case 1: return nodes[0];
				default:

					var retval = LocateNode(GetKeyHash(key));

					// if the result is not alive then try to mutate the item key and 
					// find another node this way we do not have to reinitialize every 
					// time a node dies/comes back
					// (DefaultServerPool will resurrect the nodes in the background without affecting the hashring)
					if (!retval.IsAlive)
					{
						for (var i = 0; i < nodes.Length; i++)
						{
							// -- this is from spymemcached
							var tmpKey = (ulong)GetKeyHash(i + key);
							tmpKey += (uint)(tmpKey ^ (tmpKey >> 32));
							tmpKey &= 0xffffffffL; /* truncate to 32-bits */
							// -- end
							retval = LocateNode((uint)tmpKey);

							if (retval.IsAlive) return retval;
						}
					}

					return retval;
			}
		}

		private INode LocateNode(uint itemKeyHash)
		{
			// get the index of the server assigned to this hash
			var foundIndex = Array.BinarySearch<uint>(keyRing, itemKeyHash);

			// no exact match
			if (foundIndex < 0)
			{
				// this is the nearest server in the list
				foundIndex = ~foundIndex;

				// it's smaller than everything, so use the last server (with the highest key)
				if (foundIndex == 0)
					foundIndex = keyRing.Length;

				// the key was larger than all server keys, so return the first server
				if (foundIndex >= keyRing.Length)
					foundIndex = 0;
			}

			return keyToServer[keyRing[foundIndex]];
		}

		#region [ FailedNode                   ]

		private class O : INode
		{
			public static readonly INode Instance;
			private static readonly TaskCompletionSource<bool> FailedTask;

			static O()
			{
				FailedTask = new TaskCompletionSource<bool>();
				FailedTask.SetException(new IOException("AlwaysDead"));
				Instance = new O();
			}

			public void Connect(bool reset, CancellationToken token) { }
			public void Shutdown() { }

			public bool IsAlive
			{
				get { return false; }
			}

			public System.Net.IPEndPoint EndPoint
			{
				get { return null; }
			}

			public Task Enqueue(IOperation op)
			{
				return FailedTask.Task;
			}

			public bool Send()
			{
				return false;
			}

			public bool Receive()
			{
				return false;
			}

			public override string ToString()
			{
				return "FailedNode";
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
