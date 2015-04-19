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
			: base(cluster, opFactory, keyTransformer, transcoder) { }

		public IGetOperationResult<T> Get<T>(string key)
		{
			try
			{
				return GetAsync<T>(key).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public IDictionary<string, IGetOperationResult<object>> Get(IEnumerable<string> keys)
		{
			try
			{
				return GetAsync(keys).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public async Task<IGetOperationResult<T>> GetAsync<T>(string key)
		{
			var result = await PerformGetCore(key).ConfigureAwait(false);
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

		public IGetOperationResult<T> GetAndTouch<T>(string key, DateTime expiration)
		{
			try
			{
				return GetAndTouchAsync<T>(key, expiration).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public async Task<IGetOperationResult<T>> GetAndTouchAsync<T>(string key, DateTime expiration)
		{
			var result = await PerformGetAndTouchCore(key, GetExpiration(expiration)).ConfigureAwait(false);
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

		public Task<IOperationResult> TouchAsync(string key, DateTime expiration)
		{
			return PerformTouch(key, GetExpiration(expiration));
		}

		public IOperationResult Store(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			try
			{
				return PerformStoreAsync(mode, key, value, cas, GetExpiration(expiresAt)).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			return PerformStoreAsync(mode, key, value, cas, GetExpiration(expiresAt));
		}

		public IOperationResult Remove(string key, ulong cas)
		{
			try
			{
				return PerformRemove(key, cas).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public Task<IOperationResult> RemoveAsync(string key, ulong cas)
		{
			return PerformRemove(key, cas);
		}

		public IOperationResult Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			try
			{
				return PerformConcate(mode, key, cas, data).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public Task<IOperationResult> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return PerformConcate(mode, key, cas, data);
		}

		public IMutateOperationResult Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			try
			{
				return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(expiresAt)).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(expiresAt));
		}

		public IOperationResult FlushAll()
		{
			try
			{
				return PerformFlushAll().Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public Task<IOperationResult> FlushAllAsync()
		{
			return PerformFlushAll();
		}

		public IStatsOperationResult Stats(string key)
		{
			try
			{
				return PerformStats(key).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
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
