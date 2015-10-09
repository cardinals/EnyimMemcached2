using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsExtensionsTests
	{
		[Fact]
		public void ReplaceAsync_NoExpiration_NoCas()
		{
			Verify(c => c.ReplaceAsync(Key, Value),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void ReplaceAsync_NoExpiration()
		{
			Verify(c => c.ReplaceAsync(Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void ReplaceAsync_NoCas()
		{
			Verify(c => c.ReplaceAsync(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, HasExpiration, NoCas));
		}

		[Fact]
		public void Replace_NoExpiration_NoCas()
		{
			Verify(c => c.Replace(Key, Value),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void Replace_NoExpiration()
		{
			Verify(c => c.Replace(Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void Replace_NoCas()
		{
			Verify(c => c.Replace(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, HasExpiration, NoCas));
		}
	}
}
