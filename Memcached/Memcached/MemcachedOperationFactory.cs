using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching.Memcached
{
	public class MemcachedOperationFactory : IOperationFactory
	{
		public IGetOperation Get(string key)
		{
			return new GetOperation(key);
		}

		public IStoreOperation Store(StoreMode mode, string key, CacheItem value, ulong cas, uint expires)
		{
			return new StoreOperation(mode, key, value, expires) { Cas = cas };
		}

		public IDeleteOperation Delete(string key, ulong cas)
		{
			return new DeleteOperation(key) { Cas = cas };
		}

		public IMutateOperation Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			return new MutateOperation(mode, key, defaultValue, delta, expires) { Cas = cas };
		}

		public IConcatOperation Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
		{
			return new ConcatOperation(mode, key, data) { Cas = cas };
		}

		public IFlushOperation Flush()
		{
			return new FlushOperation();
		}
	}
}
