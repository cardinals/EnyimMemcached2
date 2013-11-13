using System;

namespace Enyim.Caching.Memcached
{
	public static class SystemTime
	{
		static SystemTime()
		{
			Now = () => DateTime.Now;
		}

		public static Func<DateTime> Now { get; private set; }

		public static IDisposable Set(Func<DateTime> now)
		{
			Now = now;

			return Reset.Instance;
		}

		private class Reset : IDisposable
		{
			public static readonly IDisposable Instance = new Reset();

			public void Dispose()
			{
				SystemTime.Now = () => DateTime.Now;
			}
		}
	}
}
