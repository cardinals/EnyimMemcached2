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
		public void RemoveAsync_NoCas()
		{
			Verify(c => c.RemoveAsync(Key),
					c => c.RemoveAsync(Key, NoCas));
		}

		[Fact]
		public void Remove()
		{
			Verify(c => c.Remove(Key, HasCas),
					c => c.RemoveAsync(Key, HasCas));
		}

		[Fact]
		public void Remove_NoCas()
		{
			Verify(c => c.Remove(Key),
					c => c.RemoveAsync(Key, NoCas));
		}
	}
}
