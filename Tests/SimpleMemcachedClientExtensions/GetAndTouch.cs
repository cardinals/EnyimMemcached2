using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Moq;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class SimpleMemcachedClientExtensionsTests
	{
		[Fact]
		public void GetAndTouchAsync_NoExpiration()
		{
			Verify(c => c.GetAndTouchAsync<int>(Key), c => c.GetAndTouchAsync<int>(Key, Expiration.Never));
			Verify(c => c.GetAndTouchAsync<string>(Key), c => c.GetAndTouchAsync<string>(Key, Expiration.Never));
			Verify(c => c.GetAndTouchAsync<object>(Key), c => c.GetAndTouchAsync<object>(Key, Expiration.Never));
			Verify(c => c.GetAndTouchAsync<DateTime>(Key), c => c.GetAndTouchAsync<DateTime>(Key, Expiration.Never));
		}

		[Fact]
		public void GetAndTouch_NoExpiration()
		{
			Verify(c => c.GetAndTouch<int>(Key), c => c.GetAndTouchAsync<int>(Key, Expiration.Never));
			Verify(c => c.GetAndTouch<string>(Key), c => c.GetAndTouchAsync<string>(Key, Expiration.Never));
			Verify(c => c.GetAndTouch<object>(Key), c => c.GetAndTouchAsync<object>(Key, Expiration.Never));
			Verify(c => c.GetAndTouch<DateTime>(Key), c => c.GetAndTouchAsync<DateTime>(Key, Expiration.Never));
		}

		[Fact]
		public void GetAndTouch_HasExpiration()
		{
			Verify(c => c.GetAndTouch<int>(Key, HasExpiration), c => c.GetAndTouchAsync<int>(Key, HasExpiration));
			Verify(c => c.GetAndTouch<string>(Key, HasExpiration), c => c.GetAndTouchAsync<string>(Key, HasExpiration));
			Verify(c => c.GetAndTouch<object>(Key, HasExpiration), c => c.GetAndTouchAsync<object>(Key, HasExpiration));
			Verify(c => c.GetAndTouch<DateTime>(Key, HasExpiration), c => c.GetAndTouchAsync<DateTime>(Key, HasExpiration));
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
