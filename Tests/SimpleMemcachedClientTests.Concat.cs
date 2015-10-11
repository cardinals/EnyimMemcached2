using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class SimpleMemcachedClientTests
	{
		[Fact]
		public async void When_Appending_To_Existing_Value_Result_Is_Successful()
		{
			const string ToAppend = "The End";
			var key = GetUniqueKey("Append_Success");
			var value = GetRandomString();

			Assert.True(await Store(key: key, value: value));

			Assert.True(await client.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToAppend))));
			Assert.Equal(value + ToAppend, await client.GetAsync<object>(key));
		}

		[Fact]
		public async void When_Appending_To_Invalid_Key_Result_Is_Not_Successful()
		{
			const string ToAppend = "The End";
			var key = GetUniqueKey("Append_Fail");

			Assert.False(await client.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToAppend))));
			Assert.Null(await client.GetAsync<object>(key));
		}

		[Fact]
		public async void When_Prepending_To_Existing_Value_Result_Is_Successful()
		{
			const string ToPrepend = "The End";
			var key = GetUniqueKey("Prepend_Success");
			var value = GetRandomString();

			Assert.True(await Store(key: key, value: value));

			Assert.True(await client.ConcateAsync(ConcatenationMode.Prepend, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToPrepend))));
			Assert.Equal(ToPrepend + value, await client.GetAsync<object>(key));
		}

		[Fact]
		public async void When_Prepending_To_Invalid_Key_Result_Is_Not_Successful()
		{
			const string ToPrepend = "The End";
			var key = GetUniqueKey("Prepend_Fail");

			Assert.False(await client.ConcateAsync(ConcatenationMode.Prepend, key, new ArraySegment<byte>(Encoding.UTF8.GetBytes(ToPrepend))));
			Assert.Null(await client.GetAsync<object>(key));
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
