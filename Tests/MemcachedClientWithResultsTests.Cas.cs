using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsTests
	{
		[Fact]
		public void When_Storing_Item_With_Valid_Cas_Result_Is_Successful()
		{
			var key = GetUniqueKey("Cas_Success");
			var value = GetRandomString();

			var storeResult = ShouldPass(Store(StoreMode.Add, key, value));
			ShouldPass(client.Set(key, value, storeResult.Cas));
		}

		[Fact]
		public void When_Storing_Item_With_Invalid_Cas_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Cas_Fail");
			var value = GetRandomString();

			// make sure cas > 1 (so that we can provide a non-zero cas for the last store)
			ShouldPass(Store(StoreMode.Set, key, value));
			var storeResult = ShouldPass(Store(StoreMode.Set, key, value));

			Assert.True(storeResult.Cas > 1, "Cas should be > 1");
			ShouldFail(client.Set(key, value, cas: storeResult.Cas - 1));
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
