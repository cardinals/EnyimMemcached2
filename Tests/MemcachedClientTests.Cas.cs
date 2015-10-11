using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientTests
	{
		[Fact]
		public async void When_Storing_Item_With_Valid_Cas_Result_Is_Successful()
		{
			var key = GetUniqueKey("Cas_Success");
			var value = GetRandomString();

			var storeResult = ShouldPass(await Store(StoreMode.Add, key, value));
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

			Assert.True(storeResult.Cas > 1, "Cas should be > 1");
			ShouldFail(await client.StoreAsync(StoreMode.Set, key, value, Expiration.Never, storeResult.Cas - 1));
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
