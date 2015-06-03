using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public partial class MemcachedClientWithResults : MemcachedClientBase, IMemcachedClientWithResults
	{
		public MemcachedClientWithResults() : base() { }
		public MemcachedClientWithResults(IContainer container) : base(container) { }
		public MemcachedClientWithResults(ICluster cluster, IOperationFactory opFactory, IKeyTransformer keyTransformer, ITranscoder transcoder)
			: base(cluster, opFactory, keyTransformer, transcoder)
		{ }

		public async Task<IGetOperationResult<T>> GetAsync<T>(string key, ulong cas = Protocol.NO_CAS)
		{
			var result = await PerformGetCore(key, cas).ConfigureAwait(false);
			var converted = ConvertToResult<T>(result);

			return converted;
		}

		public async Task<IDictionary<string, IGetOperationResult<object>>> GetAsync(IEnumerable<string> keys)
		{
			var ops = await MultiGetCore(keys).ConfigureAwait(false);
			var retval = new Dictionary<string, IGetOperationResult<object>>();

			foreach (var kvp in ops)
			{
				retval[kvp.Key] = ConvertToResult<object>(kvp.Value.Result);
			}

			return retval;
		}

		public async Task<IGetOperationResult<T>> GetAndTouchAsync<T>(string key, DateTime expiresAt, ulong cas = Protocol.NO_CAS)
		{
			var result = await PerformGetAndTouchCore(key, GetExpiration(expiresAt)).ConfigureAwait(false);
			var converted = ConvertToResult<T>(result);

			return converted;
		}

		public async Task<IGetOperationResult<T>> GetAndTouchAsync<T>(string key, TimeSpan validFor, ulong cas = Protocol.NO_CAS)
		{
			var result = await PerformGetAndTouchCore(key, GetExpiration(validFor)).ConfigureAwait(false);
			var converted = ConvertToResult<T>(result);

			return converted;
		}

		public IOperationResult Touch(string key, DateTime expiration)
		{
			try
			{
				return TouchAsync(key, expiration).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public Task<IOperationResult> TouchAsync(string key, DateTime expiresAt, ulong cas = Protocol.NO_CAS)
		{
			return PerformTouch(key, GetExpiration(expiresAt), cas);
		}

		public Task<IOperationResult> TouchAsync(string key, TimeSpan validFor, ulong cas = Protocol.NO_CAS)
		{
			return PerformTouch(key, GetExpiration(validFor), cas);
		}

		public Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas)
		{
			return PerformStoreAsync(mode, key, value, GetExpiration(expiresAt), cas);
		}

		public Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas)
		{
			return PerformStoreAsync(mode, key, value, GetExpiration(validFor), cas);
		}

		public Task<IOperationResult> RemoveAsync(string key, ulong cas)
		{
			return PerformRemove(key, cas);
		}

		public Task<IOperationResult> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return PerformConcate(mode, key, cas, data);
		}

		public Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, DateTime expiresAt, ulong defaultValue, ulong delta, ulong cas)
		{
			return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(expiresAt));
		}

		public Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, TimeSpan validFor, ulong defaultValue, ulong delta, ulong cas)
		{
			return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(validFor));
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
