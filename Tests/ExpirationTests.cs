using System;
using Enyim.Caching.Memcached;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class ExpirationTests
	{
		public void TestTimeSpan()
		{
			Assert.AreEqual(MC.PublicGetExpiration(TimeSpan.Zero), 0u);
			Assert.AreEqual(MC.PublicGetExpiration(TimeSpan.FromSeconds(100)), 100u);
			Assert.AreEqual(MC.PublicGetExpiration(TimeSpan.MaxValue), 0u);

			using (SystemTime.Set(() => new DateTime(2012, 1, 1)))
			{
				Assert.AreEqual(MC.PublicGetExpiration(TimeSpan.FromDays(31)), 1328050800u);
			}
		}

		[TestMethod]
		public void TestDateTime()
		{
			Assert.AreEqual(MC.PublicGetExpiration(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)), 0u);
			Assert.AreEqual(MC.PublicGetExpiration(DateTime.MaxValue), 0u);
			Assert.AreEqual(MC.PublicGetExpiration(new DateTime(2012, 2, 1)), 1328050800u);
		}
	}

	class MC : MemcachedClient
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
