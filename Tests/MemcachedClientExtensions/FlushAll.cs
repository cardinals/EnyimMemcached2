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
		public void FlushAll()
		{
			Verify(c => c.FlushAll(),
					c => c.FlushAllAsync());
		}
	}
}
