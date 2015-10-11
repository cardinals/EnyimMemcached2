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
		public void SetAsync_NoExpiration_NoCas()
		{
			Verify(c => c.SetAsync(Key, Value),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void SetAsync_NoExpiration()
		{
			Verify(c => c.SetAsync(Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void SetAsync_NoCas()
		{
			Verify(c => c.SetAsync(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Set, Key, Value, HasExpiration, NoCas));
		}

		[Fact]
		public void Set_NoExpiration_NoCas()
		{
			Verify(c => c.Set(Key, Value),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void Set_NoExpiration()
		{
			Verify(c => c.Set(Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Set, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void Set_NoCas()
		{
			Verify(c => c.Set(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Set, Key, Value, HasExpiration, NoCas));
		}
	}
}
