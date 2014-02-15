using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class MemcachedCluster : BasicCluster
	{
		private readonly Func<IPEndPoint, INode> nodeFactory;

		public MemcachedCluster(IEnumerable<IPEndPoint> endpoints, INodeLocator locator, IReconnectPolicy policy, Func<IPEndPoint, MemcachedNode> nodeFactory)
			: base(endpoints, locator, policy)
		{
			this.nodeFactory = nodeFactory;
		}

		protected override INode CreateNode(IPEndPoint endpoint)
		{
			return nodeFactory(endpoint);
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
