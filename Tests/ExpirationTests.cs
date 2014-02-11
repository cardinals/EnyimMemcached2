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
			Assert.Equal(TestClient.PublicGetExpiration(TimeSpan.Zero), 0u);
			Assert.Equal(TestClient.PublicGetExpiration(TimeSpan.FromSeconds(100)), 100u);
			Assert.Equal(TestClient.PublicGetExpiration(TimeSpan.MaxValue), 0u);

			using (SystemTime.Set(() => new DateTime(2012, 1, 1)))
			{
				Assert.Equal(TestClient.PublicGetExpiration(TimeSpan.FromDays(31)), 1328050800u);
			}
		}

		[Fact]
		public void TestDateTime()
		{
			Assert.Equal(TestClient.PublicGetExpiration(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)), 0u);
			Assert.Equal(TestClient.PublicGetExpiration(DateTime.MaxValue), 0u);
			Assert.Equal(TestClient.PublicGetExpiration(new DateTime(2012, 2, 1)), 1328050800u);
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
