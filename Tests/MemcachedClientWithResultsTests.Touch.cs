using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsTests
	{
		[Fact]
		[Trait("slow", "yes")]
		public void When_Getting_An_Expired_Item_It_Should_Be_Null()
		{
			var key = GetUniqueKey("Get_Expired");
			var value = GetRandomString();

			ShouldPass(client.Store(StoreMode.Set, key, value, expiresAt: DateTime.Now.AddMilliseconds(MemcachedClientTests.DefaultExpiration)));
			Thread.Sleep(MemcachedClientTests.WaitButStillAlive);
			ShouldPass(client.Get(key), value);

			Thread.Sleep(MemcachedClientTests.WaitUntilExpires);
			ShouldFail(client.Get(key));
		}

		[Fact]
		[Trait("slow", "yes")]
		public void When_Getting_And_Touching_An_Item_It_Should_Not_Expire()
		{
			var key = GetUniqueKey("Get_And_Touch");
			var value = GetRandomString();

			ShouldPass(client.Store(StoreMode.Set, key, value, expiresAt: DateTime.Now.AddMilliseconds(MemcachedClientTests.DefaultExpiration)));
			Thread.Sleep(MemcachedClientTests.WaitButStillAlive);
			ShouldPass(client.GetAndTouch<string>(key, DateTime.Now.AddSeconds(MemcachedClientTests.NewExpiration)), value);

			Thread.Sleep(MemcachedClientTests.WaitUntilExpires);
			ShouldPass(client.Get(key), value);
		}

		[Fact]
		[Trait("slow", "yes")]
		public void When_Touching_An_Item_It_Should_Not_Expire()
		{
			var key = GetUniqueKey("Get_And_Touch");
			var value = GetRandomString();

			ShouldPass(client.Store(StoreMode.Set, key, value, expiresAt: DateTime.Now.AddMilliseconds(MemcachedClientTests.DefaultExpiration)));
			Thread.Sleep(MemcachedClientTests.WaitButStillAlive);

			var touchResult = client.Touch(key, DateTime.Now.AddSeconds(MemcachedClientTests.NewExpiration));
			Assert.Equal(0, touchResult.StatusCode);

			Thread.Sleep(MemcachedClientTests.WaitUntilExpires);
			ShouldPass(client.Get(key), value);
		}
	}
}

#region [ License information          ]

/* ************************************************************
 *
 *    Copyright (c) Attila Kiskó, enyim.com
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
