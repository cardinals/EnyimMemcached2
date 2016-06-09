using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class LoudMemcachedClientTests : MemcachedClientTests, IClassFixture<MemcachedClientConfigFixture>
	{
		public LoudMemcachedClientTests(MemcachedClientConfigFixture fixture) : base("LoudMemcachedClientTests", fixture.Config) { }

		[Fact]
		public async void When_Appending_To_Existing_Value_Result_Is_Not_Successful_With_Invalid_Cas()
		{
			const string ToAppend = "The End";
			var key = GetUniqueKey("Append_Cas_Fail");
			var value = GetRandomString();

			// make sure cas > 1 (so that we can provide a non-zero cas for the last store)
			ShouldPass(await Store(key: key, value: value));
			var storeResult = ShouldPass(await Store(key: key, value: value));

			Assert.True(storeResult.Cas > 1, "Cas should be > 1");

			ShouldFail(await client.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToAppend)), storeResult.Cas - 1));
			AreEqual(value, await client.GetAsync<object>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Prepending_To_Existing_Value_Result_Is_Not_Successful_With_Invalid_Cas()
		{
			const string ToPrepend = "The Beginning";
			var key = GetUniqueKey("Prepend_Cas_Fail");
			var value = GetRandomString();

			// make sure cas > 1 (so that we can provide a non-zero cas for the last store)
			ShouldPass(await Store(key: key, value: value));
			var storeResult = ShouldPass(await Store(key: key, value: value));

			Assert.True(storeResult.Cas > 1, "Cas should be > 1");

			ShouldFail(await client.ConcateAsync(ConcatenationMode.Prepend, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToPrepend)), storeResult.Cas - 1));
			AreEqual(value, await client.GetAsync<object>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Storing_Item_With_Valid_Cas_Result_Is_Successful()
		{
			var key = GetUniqueKey("Cas_Success");
			var value = GetRandomString();

			var storeResult = ShouldPass(await Store(StoreMode.Add, key, value));
			Assert.True(storeResult.Cas > 0, "Cas should be > 0");

			ShouldPass(await client.StoreAsync(StoreMode.Set, key, value, Expiration.Never, storeResult.Cas));
		}

		[Fact]
		public async void When_Storing_Item_With_Invalid_Cas_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Cas_Fail");
			var value = GetRandomString();

			// make sure cas > 1 (so that we can provide a non-zero cas for the last store)
			ShouldPass(await Store(StoreMode.Set, key, value));
			var storeResult = ShouldPass(await Store(StoreMode.Set, key, value));

			Assert.True(storeResult.Cas > 0, "Cas should be > 0");
			ShouldFail(await client.StoreAsync(StoreMode.Set, key, value, Expiration.Never, storeResult.Cas - 1));
		}


		[Fact]
		public async void When_Incrementing_Value_Result_Is_Successful()
		{
			var key = GetUniqueKey("Increment");

			AreEqual(200ul, await client.MutateAsync(MutationMode.Increment, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual(210ul, await client.MutateAsync(MutationMode.Increment, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Getting_An_Incremented_Value_It_Must_Be_A_String()
		{
			var key = GetUniqueKey("Increment_Get");

			AreEqual(200ul, await client.MutateAsync(MutationMode.Increment, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual(210ul, await client.MutateAsync(MutationMode.Increment, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual("210", await client.GetAsync<string>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void Can_Increment_Value_Initialized_By_Store()
		{
			var key = GetUniqueKey("Increment_Store");

			ShouldPass(result: await Store(key: key, value: "200"));
			AreEqual(210ul, await client.MutateAsync(MutationMode.Increment, key, Expiration.Never, 10, 10, Protocol.NO_CAS));
			AreEqual("210", await client.GetAsync<string>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Decrementing_Value_Result_Is_Successful()
		{
			var key = GetUniqueKey("Decrement");

			AreEqual(200ul, await client.MutateAsync(MutationMode.Decrement, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual(190ul, await client.MutateAsync(MutationMode.Decrement, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Getting_A_Decremented_Value_It_Must_Be_A_String()
		{
			var key = GetUniqueKey("Decrement_Get");

			AreEqual(200ul, await client.MutateAsync(MutationMode.Decrement, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual(190ul, await client.MutateAsync(MutationMode.Decrement, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual("190", await client.GetAsync<string>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void Can_Decrement_Value_Initialized_By_Store()
		{
			var key = GetUniqueKey("Decrement_Store");

			ShouldPass(result: await Store(key: key, value: "200"));
			AreEqual(190ul, await client.MutateAsync(MutationMode.Decrement, key, Expiration.Never, 10, 10, Protocol.NO_CAS));
			AreEqual("190", await client.GetAsync<string>(key, Protocol.NO_CAS));
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
