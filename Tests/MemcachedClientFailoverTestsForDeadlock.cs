using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientFailoverTestsForDeadlock_2 : MemcachedClientTestBase, IDisposable
	{
		private IMemcachedClient client;
		private IContainer config;

		public MemcachedClientFailoverTestsForDeadlock_2()
		{
			var name = GetType().Name;

			// note: we're intentionally adding a dead server
			new ClusterBuilder(name)
					.Endpoints("localhost:11300")
					.SocketOpts(connectionTimeout: TimeSpan.FromMilliseconds(100))
					.Use
						.ReconnectPolicy(() => new PeriodicReconnectPolicy { Interval = TimeSpan.FromHours(1) })
					.Register();

			config = new ClientConfigurationBuilder().Cluster(name).Create();
			client = new MemcachedClient(config);
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}

		public void Dispose()
		{
			config.Dispose();
			ClusterManager.Shutdown(GetType().Name);

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

	//[Trait("debug", "yes")]
	public partial class MemcachedClientFailoverTestsForDeadlock_3 : MemcachedClientTestBase, IDisposable
	{
		private IMemcachedClient client;
		private IContainer config;

		public MemcachedClientFailoverTestsForDeadlock_3()
		{
			var name = GetType().Name;

			// note: we're intentionally adding a dead server
			new ClusterBuilder(name)
					.Endpoints("localhost:11300")
					.SocketOpts(connectionTimeout: TimeSpan.FromMilliseconds(100))
					.Use
						.ReconnectPolicy(() => new PeriodicReconnectPolicy { Interval = TimeSpan.FromHours(1) })
					.Register();

			config = new ClientConfigurationBuilder().Cluster(name).Create();
			client = new MemcachedClient(config);
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}

		public void Dispose()
		{
			config.Dispose();
			ClusterManager.Shutdown(GetType().Name);

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

	public partial class MemcachedClientFailoverTestsForDeadlock_4 : MemcachedClientTestBase, IDisposable
	{
		private IMemcachedClient client;
		private IContainer config;

		public MemcachedClientFailoverTestsForDeadlock_4()
		{
			var name = GetType().Name;

			// note: we're intentionally adding a dead server
			new ClusterBuilder(name)
					.Endpoints("localhost:11300")
					.SocketOpts(connectionTimeout: TimeSpan.FromMilliseconds(100))
					.Use
						.ReconnectPolicy(() => new PeriodicReconnectPolicy { Interval = TimeSpan.FromHours(1) })
					.Register();

			config = new ClientConfigurationBuilder().Cluster(name).Create();
			client = new MemcachedClient(config);
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}

		public void Dispose()
		{
			config.Dispose();
			ClusterManager.Shutdown(GetType().Name);

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
