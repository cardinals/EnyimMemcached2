using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientTests
	{
		public const int DefaultExpiration = 2000;
		public const int WaitButStillAlive = 500;
		public const int NewExpiration = 20000;
		public const int WaitUntilExpires = 3000;

		[Fact]
		public void When_Getting_An_Expired_Item_It_Should_Be_Null()
		{
			var key = GetUniqueKey("Get_Expired");
			var value = GetRandomString();

			Assert.True(client.Store(StoreMode.Set, key, value, DateTime.Now.AddMilliseconds(DefaultExpiration)));
			Thread.Sleep(WaitButStillAlive);
			Assert.Equal(value, client.Get(key));

			Thread.Sleep(WaitUntilExpires);
			Assert.Null(client.Get(key));

		}

		[Fact]
		public void When_Getting_And_Touching_An_Item_It_Should_Not_Expire()
		{
			var key = GetUniqueKey("Get_And_Touch");
			var value = GetRandomString();

			Assert.True(client.Store(StoreMode.Set, key, value, DateTime.Now.AddMilliseconds(DefaultExpiration)));
			Thread.Sleep(WaitButStillAlive);
			Assert.Equal(value, client.GetAndTouch<string>(key, DateTime.Now.AddMilliseconds(NewExpiration)));

			Thread.Sleep(WaitUntilExpires);
			Assert.Equal(value, client.Get(key));
		}

		[Fact]
		public void When_Touching_An_Item_It_Should_Not_Expire()
		{
			var key = GetUniqueKey("Get_And_Touch");
			var value = GetRandomString();

			Assert.True(client.Store(StoreMode.Set, key, value, DateTime.Now.AddMilliseconds(DefaultExpiration)));
			Thread.Sleep(WaitButStillAlive);
			Assert.True(client.Touch(key, DateTime.Now.AddMilliseconds(NewExpiration)));

			Thread.Sleep(WaitUntilExpires);
			Assert.Equal(value, client.Get(key));
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
