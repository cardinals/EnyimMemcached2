using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enyim.Caching.Memcached
{
	public class NullKeyTransformer : IKeyTransformer
	{
		private readonly IBufferAllocator allocator;

		public NullKeyTransformer(IBufferAllocator allocator)
		{
			this.allocator = allocator;
		}

		public virtual Key Transform(string key)
		{
			var max = Encoding.UTF8.GetMaxByteCount(key.Length);
			var buffer = allocator.Take(max);
			var count = Encoding.UTF8.GetBytes(key, 0, key.Length, buffer, 0);

			return new Key(allocator, buffer, count);
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
