﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public interface IMemcachedClient
	{
		Task<IGetOperationResult<T>> GetAsync<T>(string key, ulong cas);
		Task<IDictionary<string, IGetOperationResult<object>>> GetAsync(IEnumerable<KeyValuePair<string, ulong>> keys);

		Task<IGetOperationResult<T>> GetAndTouchAsync<T>(string key, Expiration expiration, ulong cas);
		Task<IOperationResult> TouchAsync(string key, Expiration expiration, ulong cas);

		Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, Expiration expiration, ulong cas);
		Task<IOperationResult> RemoveAsync(string key, ulong cas);

		Task<IOperationResult> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);
		Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, Expiration expiration, ulong delta, ulong defaultValue, ulong cas);

		Task<IOperationResult> FlushAllAsync();
		Task<IStatsOperationResult> StatsAsync(string key = null);
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
