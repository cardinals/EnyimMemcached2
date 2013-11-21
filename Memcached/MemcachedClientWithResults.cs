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
			return ((IMemcachedClientWithResults)this).GetAsync<T>(key).Result;
		}

		public IDictionary<string, IGetOperationResult<object>> Get(IEnumerable<string> keys)
		{
			return ((IMemcachedClientWithResults)this).GetAsync(keys).Result;
		}

		public async Task<IGetOperationResult<T>> GetAsync<T>(string key)
		{
			var result = await PerformGetCore(key);
			var converted = ConvertToResult<T>(result);

			return converted;
		}

		public async Task<IDictionary<string, IGetOperationResult<object>>> GetAsync(IEnumerable<string> keys)
		{
			var ops = await MultiGetCore(keys);
			var retval = new Dictionary<string, IGetOperationResult<object>>();

			foreach (var kvp in ops)
			{
				retval[kvp.Key] = ConvertToResult<object>(kvp.Value.Result);
			}

			return retval;
		}

		public IOperationResult Store(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			return PerformStoreAsync(mode, key, value, GetExpiration(expiresAt), cas).Result;
		}

		public Task<IOperationResult> StoreAsync(StoreMode mode, string key, object value, ulong cas, DateTime expiresAt)
		{
			return PerformStoreAsync(mode, key, value, GetExpiration(expiresAt), cas);
		}

		public IOperationResult Remove(string key, ulong cas)
		{
			return PerformRemove(key, cas).Result;
		}

		public Task<IOperationResult> RemoveAsync(string key, ulong cas)
		{
			return PerformRemove(key, cas);
		}

		public IOperationResult Concate(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return PerformConcate(mode, key, cas, data).Result;
		}

		public Task<IOperationResult> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas)
		{
			return PerformConcate(mode, key, cas, data);
		}

		public IMutateOperationResult Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(expiresAt)).Result;
		}

		public Task<IMutateOperationResult> MutateAsync(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, DateTime expiresAt)
		{
			return PerformMutate(mode, key, defaultValue, delta, cas, GetExpiration(expiresAt));
		}
	}
}
