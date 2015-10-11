using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class SimpleMemcachedClientTests
	{
		[Fact]
		public async void When_Storing_Item_With_New_Key_And_StoreMode_Add_Result_Is_Successful()
		{
			Assert.True(await Store(StoreMode.Add, key: GetUniqueKey("Add_Once")));
		}

		[Fact]
		public async void When_Storing_Item_With_Existing_Key_And_StoreMode_Add_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Add_Twice");

			Assert.True(await Store(StoreMode.Add, key: key));
			Assert.False(await Store(StoreMode.Add, key: key));
		}

		[Fact]
		public async void When_Storing_Item_With_New_Key_And_StoreMode_Replace_Result_Is_Not_Successful()
		{
			Assert.False(await Store(StoreMode.Replace, key: GetUniqueKey("New_Replace")));
		}

		[Fact]
		public async void When_Storing_Item_With_New_Key_And_StoreMode_Set_Result_Is_Successful()
		{
			Assert.True(await Store(StoreMode.Set, key: GetUniqueKey("New_Set")));
		}

		[Fact]
		public async void When_Storing_Item_With_Existing_Key_And_StoreMode_Replace_Result_Is_Successful()
		{
			var key = GetUniqueKey("Existing_Replace");

			Assert.True(await Store(StoreMode.Add, key));
			Assert.True(await Store(StoreMode.Replace, key));
		}

		[Fact]
		public async void When_Storing_Item_With_Existing_Key_And_StoreMode_Set_Result_Is_Successful()
		{
			var key = GetUniqueKey("Existing_Set");

			Assert.True(await Store(StoreMode.Add, key));
			Assert.True(await Store(StoreMode.Set, key));
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
