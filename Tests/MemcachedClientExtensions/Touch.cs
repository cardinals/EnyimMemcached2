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
		public void TouchAsync_NoExpiration_NoCas()
		{
			Verify(c => c.TouchAsync(Key),
					c => c.TouchAsync(Key, Expiration.Never, NoCas));
		}

		[Fact]
		public void TouchAsync_NoCas()
		{
			Verify(c => c.TouchAsync(Key, HasExpiration),
					c => c.TouchAsync(Key, HasExpiration, NoCas));
		}

		[Fact]
		public void TouchAsync_NoExpiration()
		{
			Verify(c => c.TouchAsync(Key, HasCas),
					c => c.TouchAsync(Key, Expiration.Never, HasCas));
		}

		[Fact]
		public void Touch_NoExpiration_NoCas()
		{
			Verify(c => c.Touch(Key),
					c => c.TouchAsync(Key, Expiration.Never, NoCas));
		}

		[Fact]
		public void Touch_NoCas()
		{
			Verify(c => c.Touch(Key, HasExpiration),
					c => c.TouchAsync(Key, HasExpiration, NoCas));
		}

		[Fact]
		public void Touch_NoExpiration()
		{
			Verify(c => c.Touch(Key, HasCas),
					c => c.TouchAsync(Key, Expiration.Never, HasCas));
		}
	}
}
