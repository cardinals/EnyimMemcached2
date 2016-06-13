using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	/// Provides a reference to an segment of bytes allocated from a buffer pool.
	/// </summary>
	[SuppressMessage("Potential Code Quality Issues", "NonReadonlyReferencedInGetHashCodeIssue:Non-readonly field referenced in 'GetHashCode()'", Justification = "Only Dispose() changes the array")]
	public struct PooledSegment : IDisposable
	{
		public static readonly PooledSegment Empty = new PooledSegment { array = new byte[0] };

		private IBufferAllocator allocator;
		private byte[] array;
		private readonly int count;

		public PooledSegment(IBufferAllocator allocator, int count)
		{
			Require.NotNull(allocator, nameof(allocator));
			Require.That(count >= 0, $"{nameof(count)} must be >= 0");

			this.allocator = allocator;
			this.array = allocator.Take(count);
			this.count = count;
		}

		public PooledSegment(IBufferAllocator allocator, byte[] array, int count)
		{
			Require.NotNull(array, nameof(array));
			Require.That(count >= 0, $"{nameof(count)} must be >= 0");

			this.allocator = allocator;
			this.array = array;
			this.count = count;
		}

		/// <summary>
		/// Unowned buffer
		/// </summary>
		public PooledSegment(byte[] array, int count)
		{
			Require.NotNull(array, "array");
			Require.That(count >= 0, "count must be >= 0");

			this.allocator = null;
			this.array = array;
			this.count = count;
		}

		/// <summary>
		/// Unowned buffer
		/// </summary>
		internal static PooledSegment Wrap(ArraySegment<byte> source)
		{
			var count = source.Count;
			if (source.Offset == 0)
				return new PooledSegment(source.Array, count);

			var retval = new PooledSegment(new byte[count], count);
			Buffer.BlockCopy(source.Array, source.Offset, retval.array, 0, count);

			return retval;
		}

		public byte[] Array { get { return array; } }
		public int Count { get { return count; } }

		public void Dispose()
		{
			if (allocator != null)
			{
				allocator.Return(array);
				allocator = null;
			}

			array = null;
		}

		public PooledSegment Clone()
		{
			var retval = new PooledSegment(allocator, array, count);

			// the clone will return the buffer to the allocator
			allocator = null;

			return retval;
		}

		public ArraySegment<byte> AsArraySegment()
		{
			return new ArraySegment<byte>(array, 0, count);
		}

		public override bool Equals(object obj)
		{
			return obj is PooledSegment && Equals((PooledSegment)obj);
		}

		public bool Equals(PooledSegment obj)
		{
			return obj.array == array && obj.count == count;
		}

		public override int GetHashCode()
		{
			return array == null
					? 0
					: array.GetHashCode() ^ count;
		}

		public static bool operator ==(PooledSegment a, PooledSegment b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(PooledSegment a, PooledSegment b)
		{
			return !a.Equals(b);
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
