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
		public void ConcateAsync_Plain_WithDefaults()
		{
			Verify(c => c.ConcateAsync(ConcatenationMode.Append, Key, PlainData),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, new ArraySegment<byte>(PlainData), NoCas));
		}

		[Fact]
		public void ConcateAsync_Plain_WithCas()
		{
			Verify(c => c.ConcateAsync(ConcatenationMode.Append, Key, PlainData, HasCas),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, new ArraySegment<byte>(PlainData), HasCas));
		}

		[Fact]
		public void ConcateAsync_NoCas()
		{
			Verify(c => c.ConcateAsync(ConcatenationMode.Append, Key, Data),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, Data, NoCas));
		}
	}
}
