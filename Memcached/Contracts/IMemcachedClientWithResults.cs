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
		Task<IGetOperationResult<T>> GetAsync<T>(string key);
		Task<IDictionary<string, IGetOperationResult<object>>> GetAsync(IEnumerable<string> keys);

		IOperationResult Store(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt);
		IOperationResult Store(StoreMode mode, string key, object value, ulong cas, TimeSpan validFor);
		Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt);
		Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, ulong cas, TimeSpan validFor);

		IOperationResult Remove(string key, ulong cas);
		Task<IOperationResult> RemoveAsync(string key, ulong cas);

		IOperationResult Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);
		Task<IOperationResult> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);

		IMutateOperationResult Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt);
		IMutateOperationResult Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, TimeSpan validFor);
		Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt);
		Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, TimeSpan validFor);
	}
}
