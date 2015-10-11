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
		public void MutateAsync_WithDefaults()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE, Protocol.NO_CAS));
		}

		[Fact]
		public void MutateAsync_NoExpire_NoCas()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, MutateDelta, MutateDefault, Protocol.NO_CAS));
		}

		[Fact]
		public void MutateAsync_NoExpire_HasCas()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key, MutateDelta, MutateDefault, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, MutateDelta, MutateDefault, HasCas));
		}

		[Fact]
		public void MutateAsync_HasExpiration_WithDefaults()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE, Protocol.NO_CAS));
		}

		[Fact]
		public void MutateAsync_HasExpiration_HasCas()
		{
			Verify(c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, cas: HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE, HasCas));
		}

		[Fact]
		public void Mutate_WithDefaults()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE, Protocol.NO_CAS));
		}

		[Fact]
		public void Mutate_NoExpire_NoCas()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, MutateDelta, MutateDefault),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, MutateDelta, MutateDefault, Protocol.NO_CAS));
		}

		[Fact]
		public void Mutate_NoExpire_HasCas()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, MutateDelta, MutateDefault, HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, Expiration.Never, MutateDelta, MutateDefault, HasCas));
		}

		[Fact]
		public void Mutate_HasExpiration_WithDefaults()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, HasExpiration),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE, Protocol.NO_CAS));
		}

		[Fact]
		public void Mutate_HasExpiration_HasCas()
		{
			Verify(c => c.Mutate(MutationMode.Increment, Key, HasExpiration, cas: HasCas),
					c => c.MutateAsync(MutationMode.Increment, Key, HasExpiration, Protocol.MUTATE_DEFAULT_DELTA, Protocol.MUTATE_DEFAULT_VALUE, HasCas));
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
