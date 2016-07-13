using System;
using System.Linq;
using System.Collections.Generic;

namespace Enyim.Caching
{
	public static class ByteBuferExtensions
	{
		public static ByteBuffer AsByteBuffer(this ArraySegment<byte> segment, IBufferAllocator allocator = null)
		{
			if (segment.Offset == 0)
				return new ByteBuffer(null, segment.Array, segment.Count);

			var target = allocator == null ? new byte[segment.Count] : allocator.Take(segment.Count);
			Buffer.BlockCopy(segment.Array, segment.Offset, target, 0, segment.Count);

			return new ByteBuffer(allocator, target, segment.Count);
		}

		public static ArraySegment<byte> AsArraySegment(this ByteBuffer buffer)
		{
			return new ArraySegment<byte>(buffer.Array, 0, buffer.Length);
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
