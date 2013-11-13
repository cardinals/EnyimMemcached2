using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public interface IMemcachedClient
	{
		T Get<T>(string key);
		IDictionary<string, object> Get(IEnumerable<string> keys);
		Task<T> GetAsync<T>(string key);
		Task<IDictionary<string, object>> GetAsync(IEnumerable<string> keys);

		bool Store(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt);
		bool Store(StoreMode mode, string key, object value, ulong cas, TimeSpan validFor);
		Task<bool> StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt);
		Task<bool> StoreAsync(StoreMode mode, string key, object value, ulong cas, TimeSpan validFor);

		bool Remove(string key, ulong cas);
		Task<bool> RemoveAsync(string key, ulong cas);

		bool Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);
		Task<bool> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas);

		ulong Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt);
		ulong Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, TimeSpan validFor);
		Task<ulong> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt);
		Task<ulong> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, TimeSpan validFor);
	}
}
