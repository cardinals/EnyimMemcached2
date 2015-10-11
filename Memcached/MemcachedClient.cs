using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public partial class MemcachedClient : MemcachedClientBase, IMemcachedClient
	{
		public MemcachedClient() : base() { }
		public MemcachedClient(IContainer container) : base(container) { }
		public MemcachedClient(ICluster cluster, IOperationFactory opFactory, IKeyTransformer keyTransformer, ITranscoder transcoder)
			: base(cluster, opFactory, keyTransformer, transcoder)
		{ }

		public async Task<IGetOperationResult<T>> GetAsync<T>(string key, ulong cas)
		{
			var result = await PerformGetCore(key, cas).ConfigureAwait(false);
			var converted = ConvertToResult<T>(result);

			return converted;
		}

		public async Task<IDictionary<string, IGetOperationResult<object>>> GetAsync(IEnumerable<KeyValuePair<string, ulong>> keys)
		{
			var ops = await MultiGetCore(keys).ConfigureAwait(false);

			return ConvertMultigetResults(ops);
		}

		private IDictionary<string, IGetOperationResult<object>> ConvertMultigetResults(IEnumerable<KeyValuePair<string, IGetOperation>> ops)
		{
			var retval = new Dictionary<string, IGetOperationResult<object>>();

			foreach (var kvp in ops)
				retval[kvp.Key] = ConvertToResult<object>(kvp.Value.Result);

			return retval;
		}

		public async Task<IGetOperationResult<T>> GetAndTouchAsync<T>(string key, Expiration expiration, ulong cas)
		{
			var result = await PerformGetAndTouchCore(key, expiration, cas).ConfigureAwait(false);
			var converted = ConvertToResult<T>(result);

			return converted;
		}

		public Task<IOperationResult> TouchAsync(string key, Expiration expiration, ulong cas)
		{
			return PerformTouch(key, expiration, cas);
		}

		public Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, Expiration expiration, ulong cas)
		{
			return PerformStoreAsync(mode, key, value, expiration, cas);
		}

		public Task<IOperationResult> RemoveAsync(string key, ulong cas)
		{
			return PerformRemove(key, cas);
		}

		public Task<IOperationResult> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return PerformConcate(mode, key, cas, data);
		}

		public Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, Expiration expiration, ulong delta, ulong defaultValue, ulong cas)
		{
			return PerformMutate(mode, key, expiration, delta, defaultValue, cas);
		}

		public Task<IOperationResult> FlushAllAsync()
		{
			return PerformFlushAll();
		}

		public Task<IStatsOperationResult> StatsAsync(string key)
		{
			return PerformStats(key);
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
