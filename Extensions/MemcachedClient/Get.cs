using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public static partial class MemcachedClientExtensions
	{
		public static Task<object> GetAsync(this IMemcachedClient self, string key)
		{
			return self.GetAsync<object>(key);
		}

		public static object Get(this IMemcachedClient self, string key)
		{
			return self.Get<object>(key);
		}

		public static T Get<T>(this IMemcachedClient self, string key)
		{
			return self.GetAsync<T>(key).RunAndUnwrap();
		}

		public static IDictionary<string, object> Get(this IMemcachedClient self, IEnumerable<string> keys)
		{
			return self.GetAsync(keys).RunAndUnwrap();
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
