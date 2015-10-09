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
		public void IncrementAsync_WithDefaults()
		{
			Verify(c => c.IncrementAsync(Key),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, 1, 1, 0));
		}

		[Fact]
		public void IncrementAsync_NoExpire()
		{
			Verify(c => c.IncrementAsync(Key, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Delta, Default, HasCas));
		}

		[Fact]
		public void IncrementAsync_Expire()
		{
			Verify(c => c.IncrementAsync(Key, HasExpiration, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Delta, Default, HasCas));
		}

		[Fact]
		public void Increment_WithDefaults()
		{
			Verify(c => c.Increment(Key),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, 1, 1, 0));
		}

		[Fact]
		public void Increment_NoExpire()
		{
			Verify(c => c.Increment(Key, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Delta, Default, HasCas));
		}

		[Fact]
		public void Increment_Expire()
		{
			Verify(c => c.Increment(Key, HasExpiration, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Delta, Default, HasCas));
		}
	}
}
