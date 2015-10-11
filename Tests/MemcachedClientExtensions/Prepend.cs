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

#region [ License information          ]

/* ************************************************************
 *
 *    Copyright (c) Attila Kiskó, enyim.com
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion
