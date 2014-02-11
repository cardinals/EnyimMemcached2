using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public interface IMemcachedClientWithResults
	{
		IGetOperationResult<T> Get<T>(string key);
		IDictionary<string, IGetOperationResult<object>> Get(IEnumerable<string> keys);

		IOperationResult Store(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt);
		IOperationResult Remove(string key, ulong cas);
		IOperationResult Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);
		IMutateOperationResult Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt);

		Task<IGetOperationResult<T>> GetAsync<T>(string key);
		Task<IDictionary<string, IGetOperationResult<object>>> GetAsync(IEnumerable<string> keys);

		Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt);
		Task<IOperationResult> RemoveAsync(string key, ulong cas);
		Task<IOperationResult> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);
		Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt);

		IOperationResult FlushAll();
		Task<IOperationResult> FlushAllAsync();

		IStatsOperationResult Stats(string key);
		Task<IStatsOperationResult> StatsAsync(string key);
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
