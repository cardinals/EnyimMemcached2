using System;

namespace Enyim.Caching.Memcached
{
	public interface IOperationFactory
	{
		IGetOperation Get(byte[] key);

		IStoreOperation Store(StoreMode mode, byte[] key, CacheItem value, ulong cas, uint expires);
		IDeleteOperation Delete(byte[] key, ulong cas);
		IMutateOperation Mutate(MutationMode mode, byte[] key, ulong defaultValue, ulong delta, ulong cas, uint expires);
		IConcatOperation Concat(ConcatenationMode mode, byte[] key, ulong cas, ArraySegment<byte> data);

		IStatsOperation Stats(string type);
		IFlushOperation Flush();
	}
}
