using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached.Results;
using Funq;

namespace Enyim.Caching.Memcached
{
	public partial class MemcachedClient
	{
		IGetOperationResult<T> IMemcachedClientWithResults.Get<T>(string key)
		{
			return ((IMemcachedClientWithResults)this).GetAsync<T>(key).Result;
		}

		IDictionary<string, IGetOperationResult<object>> IMemcachedClientWithResults.Get(IEnumerable<string> keys)
		{
			return ((IMemcachedClientWithResults)this).GetAsync(keys).Result;
		}

		async Task<IGetOperationResult<T>> IMemcachedClientWithResults.GetAsync<T>(string key)
		{
			var result = await PerformGetCore(key);
			var converted = ConvertToResult<T>(result);

			return converted;
		}

		async Task<IDictionary<string, IGetOperationResult<object>>> IMemcachedClientWithResults.GetAsync(IEnumerable<string> keys)
		{
			var ops = await MultiGetCore(keys);
			var retval = new Dictionary<string, IGetOperationResult<object>>();

			foreach (var kvp in ops)
			{
				retval[kvp.Key] = ConvertToResult<object>(kvp.Value.Result);
			}

			return retval;
		}

		IOperationResult IMemcachedClientWithResults.Store(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			return PerformStoreAsync(mode, key, value, GetExpiration(expiresAt), cas).Result;
		}

		Task<IOperationResult> IMemcachedClientWithResults.StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			return PerformStoreAsync(mode, key, value, GetExpiration(expiresAt), cas);
		}

		IOperationResult IMemcachedClientWithResults.Remove(string key, ulong cas)
		{
			return PerformRemove(key, cas).Result;
		}

		Task<IOperationResult> IMemcachedClientWithResults.RemoveAsync(string key, ulong cas)
		{
			return PerformRemove(key, cas);
		}

		IOperationResult IMemcachedClientWithResults.Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return PerformConcate(mode, key, cas, data).Result;
		}

		Task<IOperationResult> IMemcachedClientWithResults.ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return PerformConcate(mode, key, cas, data);
		}

		IMutateOperationResult IMemcachedClientWithResults.Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(expiresAt)).Result;
		}

		Task<IMutateOperationResult> IMemcachedClientWithResults.MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(expiresAt));
		}
	}
}
