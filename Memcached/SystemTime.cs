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
