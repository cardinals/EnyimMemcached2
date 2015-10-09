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
		public void DecrementAsync_WithDefaults()
		{
			Verify(c => c.DecrementAsync(Key),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, 1, 1, 0));
		}

		[Fact]
		public void DecrementAsync_NoExpire()
		{
			Verify(c => c.DecrementAsync(Key, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, Delta, Default, HasCas));
		}

		[Fact]
		public void DecrementAsync_Expire()
		{
			Verify(c => c.DecrementAsync(Key, HasExpiration, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Decrement, Key, HasExpiration, Delta, Default, HasCas));
		}

		[Fact]
		public void Decrement_WithDefaults()
		{
			Verify(c => c.Decrement(Key),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, 1, 1, 0));
		}

		[Fact]
		public void Decrement_NoExpire()
		{
			Verify(c => c.Decrement(Key, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, Delta, Default, HasCas));
		}

		[Fact]
		public void Decrement_Expire()
		{
			Verify(c => c.Decrement(Key, HasExpiration, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Decrement, Key, HasExpiration, Delta, Default, HasCas));
		}
	}
}
