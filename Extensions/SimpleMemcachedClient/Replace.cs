using System;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public static partial class SimpleMemcachedClientExtensions
	{
		public static Task<bool> ReplaceAsync(this ISimpleMemcachedClient self, string key, object value)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, Expiration.Never);
		}

		public static Task<bool> ReplaceAsync(this ISimpleMemcachedClient self, string key, object value, Expiration expiration)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, expiration);
		}

		public static bool Replace(this ISimpleMemcachedClient self, string key, object value)
		{
			return self.Store(StoreMode.Replace, key, value, Expiration.Never);
		}

		public static bool Replace(this ISimpleMemcachedClient self, string key, object value, Expiration expiration)
		{
			return self.Store(StoreMode.Replace, key, value, expiration);
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
