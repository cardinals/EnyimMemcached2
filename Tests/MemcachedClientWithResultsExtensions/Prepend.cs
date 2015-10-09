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
		public void PrependAsync_Plain_WithDefaults()
		{
			Verify(c => c.PrependAsync(Key, PlainData),
					c => c.ConcateAsync(ConcatenationMode.Prepend, Key, new ArraySegment<byte>(PlainData), NoCas));
		}

		[Fact]
		public void PrependAsync_Plain_WithCas()
		{
			Verify(c => c.PrependAsync(Key, PlainData, HasCas),
					c => c.ConcateAsync(ConcatenationMode.Prepend, Key, new ArraySegment<byte>(PlainData), HasCas));
		}

		[Fact]
		public void PrependAsync_NoCas()
		{
			Verify(c => c.PrependAsync(Key, Data),
					c => c.ConcateAsync(ConcatenationMode.Prepend, Key, Data, NoCas));
		}

		[Fact]
		public void Prepend_Plain_WithDefaults()
		{
			Verify(c => c.Prepend(Key, PlainData),
					c => c.ConcateAsync(ConcatenationMode.Prepend, Key, new ArraySegment<byte>(PlainData), NoCas));
		}

		[Fact]
		public void Prepend_Plain_WithCas()
		{
			Verify(c => c.Prepend(Key, PlainData, HasCas),
					c => c.ConcateAsync(ConcatenationMode.Prepend, Key, new ArraySegment<byte>(PlainData), HasCas));
		}

		[Fact]
		public void Prepend_NoCas()
		{
			Verify(c => c.Prepend(Key, Data),
					c => c.ConcateAsync(ConcatenationMode.Prepend, Key, Data, NoCas));
		}
	}
}
