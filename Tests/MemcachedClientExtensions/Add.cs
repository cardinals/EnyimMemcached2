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
		public void AddAsync_NoExpiration_NoCas()
		{
			Verify(c => c.AddAsync(Key, Value),
					c => c.StoreAsync(StoreMode.Add, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void AddAsync_NoExpiration()
		{
			Verify(c => c.AddAsync(Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Add, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void AddAsync_NoCas()
		{
			Verify(c => c.AddAsync(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Add, Key, Value, HasExpiration, NoCas));
		}

		[Fact]
		public void Add_NoExpiration_NoCas()
		{
			Verify(c => c.Add(Key, Value),
					c => c.StoreAsync(StoreMode.Add, Key, Value, Expiration.Never, NoCas));
		}

		[Fact]
		public void Add_NoExpiration()
		{
			Verify(c => c.Add(Key, Value, HasCas),
					c => c.StoreAsync(StoreMode.Add, Key, Value, Expiration.Never, HasCas));
		}

		[Fact]
		public void Add_NoCas()
		{
			Verify(c => c.Add(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Add, Key, Value, HasExpiration, NoCas));
		}
	}
}
