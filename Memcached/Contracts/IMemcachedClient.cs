using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public interface IMemcachedClient
	{
		T Get<T>(string key);
		IDictionary<string, object> Get(IEnumerable<string> keys);

		bool Store(StoreMode mode, string key, object value, ulong cas, DateTime expires);
		bool Remove(string key, ulong cas);
		bool Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);
		ulong Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expires);

		Task<T> GetAsync<T>(string key);
		Task<IDictionary<string, object>> GetAsync(IEnumerable<string> keys);

		Task<bool> StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expires);
		Task<bool> RemoveAsync(string key, ulong cas);
		Task<bool> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);
		Task<ulong> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expires);

		ServerStats Stats(string key);
		Task<ServerStats> StatsAsync(string key);

		bool FlushAll();
		Task<bool> FlushAllAsync();
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
