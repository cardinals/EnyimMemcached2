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
		public void AppendAsync_Plain_WithDefaults()
		{
			Verify(c => c.AppendAsync(Key, PlainData),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, new ArraySegment<byte>(PlainData), NoCas));
		}

		[Fact]
		public void AppendAsync_Plain_WithCas()
		{
			Verify(c => c.AppendAsync(Key, PlainData, HasCas),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, new ArraySegment<byte>(PlainData), HasCas));
		}

		[Fact]
		public void AppendAsync_NoCas()
		{
			Verify(c => c.AppendAsync(Key, Data),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, Data, NoCas));
		}

		[Fact]
		public void Append_Plain_WithDefaults()
		{
			Verify(c => c.AppendAsync(Key, PlainData),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, new ArraySegment<byte>(PlainData), NoCas));
		}

		[Fact]
		public void Append_Plain_WithCas()
		{
			Verify(c => c.Append(Key, PlainData, HasCas),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, new ArraySegment<byte>(PlainData), HasCas));
		}

		[Fact]
		public void Append_NoCas()
		{
			Verify(c => c.Append(Key, Data),
					c => c.ConcateAsync(ConcatenationMode.Append, Key, Data, NoCas));
		}
	}
}
