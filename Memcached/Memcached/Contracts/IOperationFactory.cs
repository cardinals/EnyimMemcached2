using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public interface IOperationFactory
	{
		IGetOperation Get(string key);

		IStoreOperation Store(StoreMode mode, string key, CacheItem value, uint expires, ulong cas);
		IDeleteOperation Delete(string key, ulong cas);
		IMutateOperation Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, uint expires, ulong cas);
		IConcatOperation Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data);

		//IStatsOperation Stats(string type);
		IFlushOperation Flush();
	}
}
