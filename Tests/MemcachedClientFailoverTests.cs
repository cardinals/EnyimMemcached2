using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientFailoverTests : MemcachedClientTestBase, IDisposable
	{
		private const string TestName = "MemcachedClientFailoverTests";

		private IContainer config;
		private IMemcachedClient client;

		public MemcachedClientFailoverTests()
			: base(TestName)
		{
			// note: we're intentionally adding a dead server
			new ClusterBuilder(TestName)
					.Endpoints("localhost:11300")
					.SocketOpts(connectionTimeout: TimeSpan.FromMilliseconds(100))
					.Use
						.ReconnectPolicy(() => new PeriodicReconnectPolicy { Interval = TimeSpan.FromHours(1) })
					.Register();

			config = new ClientConfigurationBuilder().Cluster(TestName).Create();
			client = new MemcachedClient(config);
		}

		public void Dispose()
		{
			config.Dispose();
			ClusterManager.Shutdown(TestName);
		}

		[Fact]
		public async void Store_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			IfThrows<IOException>(await client.StoreAsync(StoreMode.Set, GetUniqueKey(), "store", 0, 0));
		}

		[Fact]
		public async void Remove_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			IfThrows<IOException>(await client.RemoveAsync(GetUniqueKey(), 0));
		}

		[Fact]
		public async void Concate_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			IfThrows<IOException>(await client.ConcateAsync(ConcatenationMode.Append, GetUniqueKey(), new ArraySegment<byte>(new byte[] { 1 }), 0));
		}

		[Fact]
		public async void Mutate_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			IfThrows<IOException>(await client.MutateAsync(MutationMode.Increment, GetUniqueKey(), Expiration.Never, 100, 100, 0));
		}

		[Fact]
		public void Storing_Unserializable_Object_Should_Throw_SerializationException()
		{
			Assert.ThrowsAsync<SerializationException>(() => client.StoreAsync(StoreMode.Set, GetUniqueKey(), new Unserializable(), 0, 0));
		}

		[Fact]
		public void Storing_Unserializable_Object_Asynchronously_Should_Throw_SerializationException()
		{
			Assert.ThrowsAsync<SerializationException>(() => client.StoreAsync(StoreMode.Set, GetUniqueKey(), new Unserializable(), 0, 0));
		}

		[Fact]
		public async void StoreAsync_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			var result = await client.StoreAsync(StoreMode.Set, GetUniqueKey(), "store", 0, 0);
			IfThrows<IOException>(result);
		}

		[Fact]
		public async void RemoveAsync_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			var result = await client.RemoveAsync(GetUniqueKey(), 0);
			IfThrows<IOException>(result);
		}

		[Fact]
		public async void ConcateAsync_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			var result = await client.ConcateAsync(ConcatenationMode.Append, GetUniqueKey(), new ArraySegment<byte>(new byte[] { 1 }), 0);
			IfThrows<IOException>(result);
		}

		[Fact]
		public async void MutateAsync_Should_Fail_With_IOException_When_Target_Node_Is_Offline()
		{
			var result = await client.MutateAsync(MutationMode.Increment, GetUniqueKey(), Expiration.Never, 100, 100, 0);
			IfThrows<IOException>(result);
		}
	}

	class Unserializable { }
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
