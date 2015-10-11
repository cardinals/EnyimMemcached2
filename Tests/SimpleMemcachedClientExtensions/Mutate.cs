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
		public void MutateAsync_WithDefaults()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE));
		}

		[Fact]
		public void MutateAsync_NoExpire()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, MutateDelta, MutateDefault));
		}

		[Fact]
		public void MutateAsync_HasExpiration()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE));
		}

		[Fact]
		public void Mutate_WitDefaults()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE));
		}

		[Fact]
		public void Mutate()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, HasExpiration, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, MutateDelta, MutateDefault));
		}

		[Fact]
		public void Mutate_HasExpiration_WithDefaults()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, HasExpiration),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE));
		}

		[Fact]
		public void Mutate_NoExpire()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, MutateDelta, MutateDefault));
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
