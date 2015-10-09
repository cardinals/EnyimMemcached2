using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsTests
	{
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
