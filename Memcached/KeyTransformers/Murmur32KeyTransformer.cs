using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching.Memcached
{
	public class Murmur32KeyTransformer : NullKeyTransformer
	{
		private readonly IBufferAllocator allocator;

		public Murmur32KeyTransformer(IBufferAllocator allocator) : base(allocator)
		{
			this.allocator = allocator;
		}

		public override Key Transform(string key)
		{
			using (var byteKey = base.Transform(key))
			{
				var hash = Murmur32.ComputeHash(byteKey.Array, 0, byteKey.Length);
				var array = allocator.Take(4);
				NetworkOrderConverter.EncodeUInt32(hash, array, 0);

				return new Key(allocator, array, 4);
			}
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
