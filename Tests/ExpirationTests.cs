using System;
using Enyim.Caching.Memcached;
using Xunit;

namespace Tests
{
	public class ExpirationTests
	{
		[Fact]
		public void TestTimeSpan()
		{
			Assert.Equal(0u, TestClient.PublicGetExpiration(TimeSpan.Zero));
			Assert.Equal(100u, TestClient.PublicGetExpiration(TimeSpan.FromSeconds(100)));
			Assert.Equal(0u, TestClient.PublicGetExpiration(TimeSpan.MaxValue));

			using (SystemTime.Set(() => new DateTime(2011, 12, 31, 23, 0, 0, DateTimeKind.Utc)))
			{
				Assert.Equal(1328050800u, TestClient.PublicGetExpiration(TimeSpan.FromDays(31)));
			}
		}

		[Fact]
		public void TestDateTime()
		{
			Assert.Equal(0u, TestClient.PublicGetExpiration(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)));
			Assert.Equal(0u, TestClient.PublicGetExpiration(DateTime.MaxValue));
			Assert.Equal(1328050800u, TestClient.PublicGetExpiration(new DateTime(2012, 01, 31, 23, 0, 0, DateTimeKind.Utc)));
		}

		private class TestClient : MemcachedClient
		{
			public static uint PublicGetExpiration(DateTime dt)
			{
				return GetExpiration(dt);
			}

			public static uint PublicGetExpiration(TimeSpan ts)
			{
				return GetExpiration(ts);
			}
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
