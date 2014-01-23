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
