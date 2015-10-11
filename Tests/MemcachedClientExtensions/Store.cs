using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientExtensionsTests
	{
		[Fact]
		public void StoreAsync_NoExpiration_NoCas()
		{
			Verify(c => c.StoreAsync(StoreMode.Set, Key, Value),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void StoreAsync_NoExpiration()
		{
			Verify(c => c.StoreAsync(StoreMode.Set, Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void StoreAsync_NoCas()
		{
			Verify(c => c.StoreAsync(StoreMode.Set, Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Set, Key, Value, HasExpiration, NoCas));
		}

		[Fact]
		public void Store_NoExpiration_NoCas()
		{
			Verify(c => c.Store(StoreMode.Set, Key, Value),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void Store_NoExpiration()
		{
			Verify(c => c.Store(StoreMode.Set, Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void Store_NoCas()
		{
			Verify(c => c.Store(StoreMode.Set, Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Set, Key, Value, HasExpiration, NoCas));
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
