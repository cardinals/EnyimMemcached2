using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Enyim.Caching.Memcached
{
	public class MemcachedCluster : ClusterBase
	{
		private IBufferAllocator allocator;
		private readonly IFailurePolicy failurePolicy;
		private readonly Func<ISocket> socketFactory;

		public MemcachedCluster(IEnumerable<IPEndPoint> endpoints, IBufferAllocator allocator,
			INodeLocator locator, IReconnectPolicy reconnectPolicy, IFailurePolicy failurePolicy,
			Func<ISocket> socketFactory)
			: base(endpoints, locator, reconnectPolicy)
		{
			this.allocator = allocator;
			this.failurePolicy = failurePolicy;
			this.socketFactory = socketFactory;
		}

		protected override INode CreateNode(IPEndPoint endpoint)
		{
			return new MemcachedNode(allocator, this, endpoint, failurePolicy, socketFactory());
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
