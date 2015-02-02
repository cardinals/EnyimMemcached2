using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientFailoverTests : MemcachedClientTestBase, IDisposable
	{
		private IMemcachedClient client;
		//private IDisposable[] servers;
		private IContainer config;

		public MemcachedClientFailoverTests()
			: base("MemcachedClientFailoverTests")
		{
			//servers = new[]
			//{
			//	MemcachedServer.Run(11300),
			//	MemcachedServer.Run(11302)
			//};

			// note: we're intentionally adding a dead server
			new ClusterBuilder("MemcachedClientFailoverTests")
					.Endpoints("localhost:11300"/*, "localhost:11302", "localhost:11304"*/)
					.SocketOpts(connectionTimeout: TimeSpan.FromMilliseconds(100))
					.Use
						//.NodeLocator<PortPrefixBasedLocator>()
						.ReconnectPolicy(() => new PeriodicReconnectPolicy { Interval = TimeSpan.FromHours(1) })
					.Register();

			config = new ClientConfigurationBuilder().Cluster("MemcachedClientFailoverTests").Create();
			client = new MemcachedClient(config);
		}

		//class PortPrefixBasedLocator : INodeLocator
		//{
		//	IDictionary<int, INode> nodes;

		//	public void Initialize(IEnumerable<INode> nodes)
		//	{
		//		if (this.nodes == null)
		//			this.nodes = nodes.ToDictionary(n => n.EndPoint.Port);
		//	}

		//	public INode Locate(byte[] key)
		//	{
		//		return nodes[Int32.Parse(Encoding.UTF8.GetString(key).Split('_')[0])];
		//	}
		//}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}

		public void Dispose()
		{
			config.Dispose();
			ClusterManager.Shutdown("MemcachedClientFailoverTests");

			//foreach (var s in servers) s.Dispose();
		}

		[Fact]
		public void Store_Should_Fail()
		{
			Assert.False(Store(StoreMode.Set));
		}

		[Fact]
		public void Remove_Should_Fail()
		{
			Assert.False(client.Remove(GetUniqueKey()));
		}
	}
}

#region [ License information          ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @author Attila Kiskó <a@enyim.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2014 Attila Kiskó, enyim.com
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
