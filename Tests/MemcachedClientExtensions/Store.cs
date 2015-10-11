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
