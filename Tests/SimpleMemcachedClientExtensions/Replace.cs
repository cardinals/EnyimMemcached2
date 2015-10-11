using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class SimpleMemcachedClientExtensionsTests
	{
		[Fact]
		public void ReplaceAsync_NoExpiration()
		{
			Verify(c => c.ReplaceAsync(Key, Value),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, Expiration.Never));
		}

		[Fact]
		public void ReplaceAsync_HasExpiration()
		{
			Verify(c => c.ReplaceAsync(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, HasExpiration));
		}

		[Fact]
		public void Replace_NoExpiration()
		{
			Verify(c => c.Replace(Key, Value),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, Expiration.Never));
		}

		[Fact]
		public void Replace_HasExpiration()
		{
			Verify(c => c.Replace(Key, Value, HasExpiration),
					c => c.StoreAsync(StoreMode.Replace, Key, Value, HasExpiration));
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
