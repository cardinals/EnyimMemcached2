using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public interface IMemcachedClient
	{
		Task<T> GetAsync<T>(string key);
		Task<IDictionary<string, object>> GetAsync(IEnumerable<string> keys);

		Task<T> GetAndTouchAsync<T>(string key, Expiration expiration);
		Task<bool> TouchAsync(string key, Expiration expiration);

		Task<bool> StoreAsync(StoreMode mode, string key, object value, Expiration expiration);
		Task<bool> RemoveAsync(string key);

		Task<bool> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data);
		Task<ulong> MutateAsync(MutationMode mode, string key, Expiration expiration, ulong defaultValue, ulong delta);

		Task<bool> FlushAllAsync();
		Task<ServerStats> StatsAsync(string key);
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
