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
		public T Get<T>(string key)
		{
			return GetAsync<T>(key).Result;
		}

		public IDictionary<string, object> Get(IEnumerable<string> keys)
		{
			return GetAsync(keys).Result;
		}

		public async Task<T> GetAsync<T>(string key)
		{
			var result = await PerformGetCore(key);
			var converted = ConvertToValue(result);

			return (T)converted;
		}

		public async Task<IDictionary<string, object>> GetAsync(IEnumerable<string> keys)
		{
			var ops = await MultiGetCore(keys);
			var retval = new Dictionary<string, object>();

			foreach (var kvp in ops)
			{
				retval[kvp.Key] = ConvertToValue(kvp.Value.Result);
			}

			return retval;
		}

		public bool Store(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			return ((IMemcachedClientWithResults)this).Store(mode, key, value, cas, expiresAt).Success;
		}

		public async Task<bool> StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			var result = await ((IMemcachedClientWithResults)this).StoreAsync(mode, key, value, cas, expiresAt);

			return result.Success;
		}

		public bool Remove(string key, ulong cas)
		{
			return ((IMemcachedClientWithResults)this).Remove(key, cas).Success;
		}

		public async Task<bool> RemoveAsync(string key, ulong cas)
		{
			var result = await ((IMemcachedClientWithResults)this).RemoveAsync(key, cas);

			return result.Success;
		}

		public bool Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return ((IMemcachedClientWithResults)this).Concate(mode, key, data, cas).Success;
		}

		public async Task<bool> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			var result = await ((IMemcachedClientWithResults)this).ConcateAsync(mode, key, data, cas);

			return result.Success;
		}

		public ulong Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			return ((IMemcachedClientWithResults)this).Mutate(mode, key, defaultValue, delta, cas, expiresAt).Value;
		}

		public async Task<ulong> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			var result = await ((IMemcachedClientWithResults)this).MutateAsync(mode, key, defaultValue, delta, cas, expiresAt);

			return result.Value;
		}
	}
}
