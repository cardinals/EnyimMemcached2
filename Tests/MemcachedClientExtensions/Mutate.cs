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
		public void MutateAsync()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Delta, Default, HasCas));
		}

		[Fact]
		public void Mutate_NoExpire()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Delta, Default, HasCas));
		}

		[Fact]
		public void Mutate_Expire()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, HasExpiration, Delta, Default, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Delta, Default, HasCas));
		}
	}
}
