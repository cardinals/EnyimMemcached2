using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching.Memcached
{
	public class MemcachedOperationFactory : IOperationFactory
	{
		public IGetOperation Get(byte[] key)
		{
			return new GetOperation(key) { Silent = false };
		}

		public IGetAndTouchOperation GetAndTouch(byte[] key, uint expires)
		{
			return new GetAndTouchOperation(key, expires);
		}

		public IStoreOperation Store(StoreMode mode, byte[] key, CacheItem value, ulong cas, uint expires)
		{
			return new StoreOperation(mode, key, value, expires) { Cas = cas, Silent = false };
		}

		public IDeleteOperation Delete(byte[] key, ulong cas)
		{
			return new DeleteOperation(key) { Cas = cas, Silent = false };
		}

		public IMutateOperation Mutate(MutationMode mode, byte[] key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			return new MutateOperation(mode, key, defaultValue, delta, expires) { Cas = cas };
		}

		public ITouchOperation Touch(byte[] key, uint expires)
		{
			return new TouchOperation(key, expires);
		}

		public IConcatOperation Concat(ConcatenationMode mode, byte[] key, ulong cas, ArraySegment<byte> data)
		{
			return new ConcatOperation(mode, key, data) { Cas = cas, Silent = false };
		}

		public IFlushOperation Flush()
		{
			return new FlushOperation();
		}

		public IStatsOperation Stats(string type)
		{
			return new StatsOperation(type);
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
