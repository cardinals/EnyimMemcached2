﻿using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class SimpleMemcachedClientExtensionsTests
	{
		[Fact]
		public void DecrementAsync_WithDefaults()
		{
			Verify(c => c.DecrementAsync(Key),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, Protocol.MUTATE_DEFAULT_VALUE, Protocol.MUTATE_DEFAULT_VALUE));
		}

		[Fact]
		public void DecrementAsync_NoExpire()
		{
			Verify(c => c.DecrementAsync(Key, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, MutateDelta, MutateDefault));
		}

		[Fact]
		public void DecrementAsync_Expire()
		{
			Verify(c => c.DecrementAsync(Key, HasExpiration, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Decrement, Key, HasExpiration, MutateDelta, MutateDefault));
		}

		[Fact]
		public void Decrement_WithDefaults()
		{
			Verify(c => c.Decrement(Key),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE));
		}

		[Fact]
		public void Decrement_NoExpire()
		{
			Verify(c => c.Decrement(Key, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Decrement, Key, Expiration.Never, MutateDelta, MutateDefault));
		}

		[Fact]
		public void Decrement_Expire()
		{
			Verify(c => c.Decrement(Key, HasExpiration, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Decrement, Key, HasExpiration, MutateDelta, MutateDefault));
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
