using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching.Memcached.Results;
using Xunit;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsTests
	{
		[Fact]
		public void When_Getting_Existing_Item_Value_Is_Not_Null_And_Result_Is_Successful()
		{
			var key = GetUniqueKey("Get_Existing");
			var value = GetRandomString();

			ShouldPass(Store(key: key, value: value));
			ShouldPass(client.Get(key), value);
		}

		[Fact]
		public void When_Getting_Item_For_Invalid_Key_HasValue_Is_False_And_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Get_Invalid");
			var getResult = client.Get<object>(key);

			Assert.Equal((int)StatusCode.KeyNotFound, getResult.StatusCode);
			ShouldFail(getResult);
		}

		[Fact]
		public void When_Generic_Getting_Existing_Item_Value_Is_Not_Null_And_Result_Is_Successful()
		{
			var key = GetUniqueKey("Generic_Get");
			var value = GetRandomString();

			ShouldPass(Store(key: key, value: value));
			ShouldPass(client.Get<string>(key), value);
		}

		[Fact]
		public void When_Getting_Multiple_Keys_Result_Is_Successful()
		{
			var keys = GetUniqueKeys().Distinct().ToArray();
			foreach (var key in keys)
			{
				ShouldPass(Store(key: key, value: "Value for" + key));
			}

			var dict = client.Get(keys);
			Assert.Equal(keys.OrderBy(_ => _), dict.Keys.OrderBy(_ => _));
			Assert.True(dict.All(kvp => kvp.Value.Success));
		}

		[Fact]
		public void When_Getting_Byte_Result_Is_Successful()
		{
			const byte expectedValue = 1;
			var key = GetUniqueKey("Get_Byte");

			ShouldPass(Store(key: key, value: expectedValue));
			ShouldPass(client.Get(key), expectedValue);
		}

		[Fact]
		public void When_Getting_SByte_Result_Is_Successful()
		{
			const sbyte expectedValue = 1;
			var key = GetUniqueKey("Get_Sbyte");

			ShouldPass(Store(key: key, value: expectedValue));
			ShouldPass(client.Get(key), expectedValue);
		}
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2012 Attila Kiskó, enyim.com
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