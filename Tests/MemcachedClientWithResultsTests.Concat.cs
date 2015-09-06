using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsTests
	{
		[Fact]
		public async void When_Appending_To_Existing_Value_Result_Is_Successful()
		{
			const string ToAppend = "The End";
			var key = GetUniqueKey("Append_Success");
			var value = GetRandomString();

			ShouldPass(await Store(key: key, value: value));
			ShouldPass(await client.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToAppend)), Protocol.NO_CAS));
			AreEqual(value + ToAppend, await client.GetAsync<object>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Appending_To_Invalid_Key_Result_Is_Not_Successful()
		{
			const string ToAppend = "The End";
			var key = GetUniqueKey("Append_Fail");

			ShouldFail(await client.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToAppend)), Protocol.NO_CAS));
			ShouldFail(await client.GetAsync<object>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Prepending_To_Existing_Value_Result_Is_Successful()
		{
			const string ToPrepend = "The Beginning";
			var key = GetUniqueKey("Prepend_Success");
			var value = GetRandomString();

			ShouldPass(await Store(key: key, value: value));
			ShouldPass(await client.ConcateAsync(ConcatenationMode.Prepend, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToPrepend)), Protocol.NO_CAS));
			AreEqual(ToPrepend + value, await client.GetAsync<object>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Prepending_To_Invalid_Key_Result_Is_Not_Successful()
		{
			const string ToPrepend = "The Beginning";
			var key = GetUniqueKey("Prepend_Fail");

			ShouldFail(await client.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToPrepend)), Protocol.NO_CAS));
			ShouldFail(await client.GetAsync<object>(key, Protocol.NO_CAS));
		}

		[Fact]
		public async void When_Appending_To_Existing_Value_Result_Is_Successful_With_Valid_Cas()
		{
			const string ToAppend = "The End";
			var key = GetUniqueKey("Append_Cas_Success");
			var value = GetRandomString();

			var storeResult = ShouldPass(await Store(key: key, value: value));
			ShouldPass(await client.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToAppend)), storeResult.Cas));
			AreEqual(value + ToAppend, await client.GetAsync<object>(key, Protocol.NO_CAS));
		}

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
		public async void When_Prepending_To_Existing_Value_Result_Is_Successful_With_Valid_Cas()
		{
			const string ToPrepend = "The Beginning";
			var key = GetUniqueKey("Prepend_Cas_Success");
			var value = GetRandomString();

			var storeResult = ShouldPass(await Store(key: key, value: value));
			ShouldPass(await client.ConcateAsync(ConcatenationMode.Prepend, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToPrepend)), storeResult.Cas));
			AreEqual(ToPrepend + value, await client.GetAsync<object>(key, Protocol.NO_CAS));
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
