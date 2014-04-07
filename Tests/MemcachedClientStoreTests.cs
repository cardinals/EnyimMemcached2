using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Tests
{
	public class MemcachedClientStoreTests : MemcachedClientTestsBase
	{
		[Fact]
		public void When_Storing_Item_With_New_Key_And_StoreMode_Add_Result_Is_Successful()
		{
			ShouldPass(Store(StoreMode.Add, key: GetUniqueKey("Add_Once")));
		}

		[Fact]
		public void When_Storing_Item_With_Existing_Key_And_StoreMode_Add_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Add_Twice");

			ShouldPass(Store(StoreMode.Add, key));
			ShouldFail(Store(StoreMode.Add, key));
		}

		[Fact]
		public void When_Storing_Item_With_New_Key_And_StoreMode_Replace_Result_Is_Not_Successful()
		{
			var result = Store(StoreMode.Replace, key:GetUniqueKey( "New_Replace"));

			Assert.Equal((int)StatusCode.KeyNotFound, result.StatusCode);
			ShouldFail(result);
		}

		[Fact]
		public void When_Storing_Item_With_New_Key_And_StoreMode_Set_Result_Is_Successful()
		{
			ShouldPass(Store(StoreMode.Set, key: GetUniqueKey("New_Set")));
		}

		[Fact]
		public void When_Storing_Item_With_Existing_Key_And_StoreMode_Replace_Result_Is_Successful()
		{
			var key = GetUniqueKey("Existing_Replace");

			ShouldPass(Store(StoreMode.Add, key));
			ShouldPass(Store(StoreMode.Replace, key));
		}

		[Fact]
		public void When_Storing_Item_With_Existing_Key_And_StoreMode_Set_Result_Is_Successful()
		{
			var key = GetUniqueKey("Existing_Set");

			ShouldPass(Store(StoreMode.Add, key));
			ShouldPass(Store(StoreMode.Set, key));
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
