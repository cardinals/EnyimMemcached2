using System;

namespace Enyim.Caching
{
	public struct Key : IDisposable, IEquatable<Key>
	{
		private IBufferAllocator owner;

		public readonly int Length;
		public readonly byte[] Array;

		public Key(IBufferAllocator owner, byte[] array, int length)
		{
			Require.NotNull(array, nameof(array));
			Require.Value(nameof(length), length >= 0, $"{nameof(length)} must be >= 0");
			Require.Value(nameof(length), length <= array.Length, $"{nameof(length)} cannot be greater than the size of the array");

			this.owner = owner;
			Array = array;
			Length = length;
		}

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
			return obj is Key && Equals((Key)obj);
		}

		public bool Equals(Key obj)
		{
			return obj.Array == Array && obj.Length == Length;
		}

		public override int GetHashCode()
		{
			return Array == null
					? 0
					: Array.GetHashCode() ^ Length;
		}

		public static bool operator ==(Key a, Key b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Key a, Key b)
		{
			return !a.Equals(b);
		}

		public Key Clone()
		{
			var retval = new Key(owner, Array, Length);
			owner = null;

			return retval;
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
