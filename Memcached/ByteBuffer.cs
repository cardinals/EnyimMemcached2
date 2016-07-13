using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Enyim.Caching
{
	public struct ByteBuffer : IDisposable, IEquatable<ByteBuffer>
	{
		public static readonly ByteBuffer Empty = new ByteBuffer(null, new byte[0], 0);

		private IBufferAllocator owner;

		public ByteBuffer(IBufferAllocator owner, byte[] array, int length)
		{
			Require.NotNull(array, nameof(array));
			Require.That(length >= 0, $"{nameof(length)} must be >= 0");

			this.owner = owner;
			Array = array;
			Length = length;
		}

		public readonly int Length;
		public readonly byte[] Array;

		public void Dispose()
		{
			if (owner != null)
			{
				owner.Return(Array);
				owner = null;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ByteBuffer && Equals((ByteBuffer)obj);
		}

		public bool Equals(ByteBuffer obj)
		{
			return obj.Array == Array && obj.Length == Length;
		}

		public override int GetHashCode()
		{
			return Array == null
					? 0
					: Array.GetHashCode() ^ Length;
		}

		public ByteBuffer Clone()
		{
			var retval = new ByteBuffer(owner, Array, Length);
			owner = null;

			return retval;
		}

		public static ByteBuffer Allocate(IBufferAllocator allocator, int length)
		{
			return new ByteBuffer(allocator, allocator.Take(length), length);
		}

		public static bool operator ==(ByteBuffer a, ByteBuffer b) => a.Equals(b);
		public static bool operator !=(ByteBuffer a, ByteBuffer b) => !a.Equals(b);
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
