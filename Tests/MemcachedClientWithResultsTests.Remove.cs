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
		public void When_Removing_A_Valid_Key_Result_Is_Successful()
		{
			var key = GetUniqueKey("Remove_Valid");

			ShouldPass(Store(key: key));
			ShouldPass(client.Remove(key), checkCas: false);
			ShouldFail(client.Get(key));
		}

		[Fact]
		public void When_Removing_An_Invalid_Key_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Remove_Invalid");

			ShouldFail(client.Get(key)); // sanity-check
			ShouldFail(client.Remove(key));
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
