using System;
using System.Text;

namespace Enyim.Caching.Memcached
{
	public static class NetworkOrderConverter
	{
		public static byte[] EncodeKey(string key)
		{
			if (String.IsNullOrEmpty(key)) return null;

			return Encoding.UTF8.GetBytes(key);
		}

		public static Key EncodeKey(IBufferAllocator allocator, string key)
		{
			if (String.IsNullOrEmpty(key)) return Key.Empty;

			var max = Encoding.UTF8.GetMaxByteCount(key.Length);
			var buffer = allocator.Take(max);
			var count = Encoding.UTF8.GetBytes(key, 0, key.Length, buffer, 0);

			return new Key(allocator, buffer, count);
		}

		public static string DecodeKey(byte[] data)
		{
			if (data == null || data.Length == 0) return null;

			return Encoding.UTF8.GetString(data, 0, data.Length);
		}

		public static string DecodeKey(byte[] data, int index, int count)
		{
			if (data == null || data.Length == 0 || count == 0) return null;

			return Encoding.UTF8.GetString(data, index, count);
		}

		public static unsafe void EncodeUInt16(uint value, byte[] buffer, int offset)
		{
			fixed (byte* bp = buffer)
			{
				bp[offset + 0] = (byte)(value >> 8);
				bp[offset + 1] = (byte)(value);
			}
		}

		public static unsafe void EncodeUInt32(uint value, byte[] buffer, int offset)
		{
			fixed (byte* bp = buffer)
			{
				bp[offset + 0] = (byte)(value >> 24);
				bp[offset + 1] = (byte)(value >> 16);
				bp[offset + 2] = (byte)(value >> 8);
				bp[offset + 3] = (byte)(value);
			}
		}

		public static unsafe void EncodeInt32(int value, byte[] buffer, int offset)
		{
			fixed (byte* bp = buffer)
			{
				bp[offset + 0] = (byte)(value >> 24);
				bp[offset + 1] = (byte)(value >> 16);
				bp[offset + 2] = (byte)(value >> 8);
				bp[offset + 3] = (byte)(value);
			}
		}

		public static unsafe void EncodeUInt64(ulong value, byte[] buffer, int offset)
		{
			fixed (byte* bp = buffer)
			{
				bp[offset + 0] = (byte)(value >> 56);
				bp[offset + 1] = (byte)(value >> 48);
				bp[offset + 2] = (byte)(value >> 40);
				bp[offset + 3] = (byte)(value >> 32);
				bp[offset + 4] = (byte)(value >> 24);
				bp[offset + 5] = (byte)(value >> 16);
				bp[offset + 6] = (byte)(value >> 8);
				bp[offset + 7] = (byte)(value);
			}
		}

		public static unsafe ushort DecodeUInt16(byte[] buffer, int offset)
		{
			fixed (byte* bp = buffer)
			{
				return (ushort)((bp[offset] << 8) + bp[offset + 1]);
			}
		}

		public static unsafe int DecodeInt32(byte[] buffer, int offset)
		{
			fixed (byte* bp = buffer)
			{
				return (bp[offset] << 24) | (bp[offset + 1] << 16) | (bp[offset + 2] << 8) | bp[offset + 3];
			}
		}

		public static unsafe ulong DecodeUInt64(byte[] buffer, int offset)
		{
			fixed (byte* bp = buffer)
			{
				var part1 = (uint)((bp[offset + 0] << 24) | (bp[offset + 1] << 16) | (bp[offset + 2] << 8) | bp[offset + 3]);
				var part2 = (uint)((bp[offset + 4] << 24) | (bp[offset + 5] << 16) | (bp[offset + 6] << 8) | bp[offset + 7]);

				return ((ulong)part1 << 32) | part2;
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
