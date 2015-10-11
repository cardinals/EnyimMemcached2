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
		public void StatsAsync_WithDefaults()
		{
			Verify(c => c.StatsAsync(),
					c => c.StatsAsync(null));
		}

		[Fact]
		public void Stats_WithDefaults()
		{
			Verify(c => c.Stats(),
					c => c.StatsAsync(null));
		}

		[Fact]
		public void Stats()
		{
			Verify(c => c.Stats(Key),
					c => c.StatsAsync(Key));
		}
	}
}
