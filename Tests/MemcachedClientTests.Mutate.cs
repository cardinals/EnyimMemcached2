using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientTests
	{
		[Fact]
		public void When_Incrementing_Value_Result_Is_Successful()
		{
			var key = GetUniqueKey("Increment");

			Assert.Equal(200u, client.Increment(key, 200, 10));
			Assert.Equal(210u, client.Increment(key, 200, 10));
		}

		[Fact]
		public void When_Getting_An_Incremented_Value_It_Must_Be_A_String()
		{
			var key = GetUniqueKey("Increment_Get");

			Assert.Equal(200u, client.Increment(key, 200, 10));
			Assert.Equal(210u, client.Increment(key, 200, 10));
			Assert.Equal("210", client.Get(key));
		}

		[Fact]
		public void Can_Increment_Value_Initialized_By_Store()
		{
			var key = GetUniqueKey("Increment_Store");

			Assert.True(client.Set(key, "200"));
			Assert.Equal(210u, client.Increment(key, 10, 10));
			Assert.Equal("210", client.Get(key));
		}

		[Fact]
		public void When_Decrementing_Value_Result_Is_Successful()
		{
			var key = GetUniqueKey("Decrement");

			Assert.Equal(200u, client.Decrement(key, 200, 10));
			Assert.Equal(190u, client.Decrement(key, 200, 10));
		}

		[Fact]
		public void When_Getting_A_Decremented_Value_It_Must_Be_A_String()
		{
			var key = GetUniqueKey("Decrement_Get");

			Assert.Equal(200u, client.Decrement(key, 200, 10));
			Assert.Equal(190u, client.Decrement(key, 200, 10));
			Assert.Equal("190", client.Get(key));
		}

		[Fact]
		public void Can_Decrement_Value_Initialized_By_Store()
		{
			var key = GetUniqueKey("Decrement_Store");

			Assert.True(client.Set(key, "200"));
			Assert.Equal(190u, client.Decrement(key, 10, 10));
			Assert.Equal("190", client.Get(key));
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
