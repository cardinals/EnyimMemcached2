using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Moq;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientExtensionsTests
	{
		[Fact]
		public void GetAsync_NoCas()
		{
			Verify(c => c.GetAsync<int>(Key), c => c.GetAsync<int>(Key, NoCas));
			Verify(c => c.GetAsync<string>(Key), c => c.GetAsync<string>(Key, NoCas));
			Verify(c => c.GetAsync<object>(Key), c => c.GetAsync<object>(Key, NoCas));
			Verify(c => c.GetAsync<DateTime>(Key), c => c.GetAsync<DateTime>(Key, NoCas));
		}

		[Fact]
		public void GetAsyncObject_NoCas()
		{
			Verify(c => c.GetAsync(Key),
					c => c.GetAsync<object>(Key, NoCas));
		}

		[Fact]
		public void GetAsyncObject_WithCas()
		{
			Verify(c => c.GetAsync(Key, HasCas),
					c => c.GetAsync<object>(Key, HasCas));
		}

		[Fact]
		public void Get_NoCas()
		{
			Verify(c => c.Get<int>(Key), c => c.GetAsync<int>(Key, NoCas));
			Verify(c => c.Get<string>(Key), c => c.GetAsync<string>(Key, NoCas));
			Verify(c => c.Get<object>(Key), c => c.GetAsync<object>(Key, NoCas));
			Verify(c => c.Get<DateTime>(Key), c => c.GetAsync<DateTime>(Key, NoCas));
		}

		[Fact]
		public void Get_WithCas()
		{
			Verify(c => c.Get<int>(Key, HasCas), c => c.GetAsync<int>(Key, HasCas));
			Verify(c => c.Get<string>(Key, HasCas), c => c.GetAsync<string>(Key, HasCas));
			Verify(c => c.Get<object>(Key, HasCas), c => c.GetAsync<object>(Key, HasCas));
			Verify(c => c.Get<DateTime>(Key, HasCas), c => c.GetAsync<DateTime>(Key, HasCas));
		}

		[Fact]
		public void MultiGet_NoCas()
		{
			var keys = Enumerable.Range(1, 10).Select(i => "key-" + i).ToArray();

			Verify(c => c.Get(keys),
					c => c.GetAsync(It.Is<IEnumerable<KeyValuePair<string, ulong>>>(v => v.Select(i => i.Key).SequenceEqual(keys))));
		}
	}
}
