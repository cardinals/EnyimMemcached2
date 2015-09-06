using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientFailoverTests : MemcachedClientTestBase, IDisposable
	{
		private IMemcachedClient client;
		private IContainer config;

		public MemcachedClientFailoverTests()
			: base("MemcachedClientFailoverTests")
		{
			// note: we're intentionally adding a dead server
			new ClusterBuilder("MemcachedClientFailoverTests")
					.Endpoints("localhost:11300")
					.SocketOpts(connectionTimeout: TimeSpan.FromMilliseconds(100))
					.Use
						.ReconnectPolicy(() => new PeriodicReconnectPolicy { Interval = TimeSpan.FromHours(1) })
					.Register();

			config = new ClientConfigurationBuilder().Cluster("MemcachedClientFailoverTests").Create();
			client = new MemcachedClient(config);
		}

		protected Task<bool> Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.StoreAsync(mode, key, value, Expiration.Never);
		}

		public void Dispose()
		{
			config.Dispose();
			ClusterManager.Shutdown("MemcachedClientFailoverTests");

			//foreach (var s in servers) s.Dispose();
		}

		[Fact]
		public async void Store_Should_Fail()
		{
			Assert.False(await Store(StoreMode.Set));
		}

		[Fact]
		public async void Remove_Should_Fail()
		{
			Assert.False(await client.RemoveAsync(GetUniqueKey()));
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
