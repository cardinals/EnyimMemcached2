using System;
using System.Runtime.InteropServices;

namespace Enyim.Caching
{
	// TODO make me internal
	public static class Murmur32
	{
		private const uint C1 = 0xcc9e2d51;
		private const uint C2 = 0x1b873593;
		private const uint CORE = 0xe6546b64;

		public static uint ComputeHash(byte[] buffer)
		{
			return ComputeHash(buffer, 0, buffer.Length);
		}

		public static uint ComputeHash(byte[] buffer, int offset, int count)
		{
			var uintBuffer = new ArrayConverter { AsByte = buffer }.AsUInt;
			var remainder = count & 3;
			var h1 = 0u;

			for (var i = offset; i < count / 4; i++)
			{
				var k1 = uintBuffer[i] * C1;
				k1 = ((k1 << 15) | (k1 >> (32 - 15))) * C2;

				h1 ^= k1;
				h1 = ((h1 << 13) | (h1 >> (32 - 13))) * 5 + CORE;
			}

			if (remainder > 0)
				h1 ^= CalcTail(buffer, offset + (count - remainder), remainder);

			h1 ^= (uint)count;

			// fmix
			h1 = (h1 ^ (h1 >> 16)) * 0x85ebca6b;
			h1 = (h1 ^ (h1 >> 13)) * 0xc2b2ae35;
			h1 ^= h1 >> 16;

			return h1;
		}

		private static uint CalcTail(byte[] source, int alignedLength, int remainder)
		{
			var k1 = 0u;

			switch (remainder)
			{
				case 3: k1 ^= (uint)source[alignedLength + 2] << 16; goto case 2;
				case 2: k1 ^= (uint)source[alignedLength + 1] << 8; goto case 1;
				case 1: k1 ^= source[alignedLength]; break;
			}

			k1 *= C1;
			k1 = ((k1 << 15) | (k1 >> (32 - 15))) * C2;

			return k1;
		}

		[StructLayout(LayoutKind.Explicit)]
		struct ArrayConverter
		{
			[FieldOffset(0)]
			public Byte[] AsByte;

			[FieldOffset(0)]
			public UInt32[] AsUInt;
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
