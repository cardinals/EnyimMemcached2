using System;
using System.Runtime.CompilerServices;
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

			for (int i = offset, max = count / 4; i < max; i++)
			{
				var k1 = uintBuffer[i] * C1;
				k1 = ((k1 << 15) | (k1 >> (32 - 15))) * C2;

				h1 ^= k1;
				h1 = ((h1 << 13) | (h1 >> (32 - 13))) * 5 + CORE;
			}

			if (remainder > 0)
			{
				var alignedLength = offset + (count - remainder);
				var tail = 0u;

				switch (remainder)
				{
					case 3: tail ^= (uint)buffer[alignedLength + 2] << 16; goto case 2;
					case 2: tail ^= (uint)buffer[alignedLength + 1] << 8; goto case 1;
					case 1: tail ^= buffer[alignedLength]; break;
				}

				tail *= C1;
				tail = ((tail << 15) | (tail >> (32 - 15))) * C2;

				h1 ^= tail;
			}

			h1 ^= (uint)count;

			// fmix
			h1 = (h1 ^ (h1 >> 16)) * 0x85ebca6b;
			h1 = (h1 ^ (h1 >> 13)) * 0xc2b2ae35;
			h1 ^= h1 >> 16;

			return h1;
		}

		// HACK PEVerify will not be able to load the parent type
		// [IL]: Error: [...\Enyim.Caching.Core.dll : Enyim.Caching.Murmur32::ComputeHash] Type load failed.
		// [token  0x02000043] Type load failed.
		[StructLayout(LayoutKind.Explicit)]
		private struct ArrayConverter
		{
			[FieldOffset(0)]
			public byte[] AsByte;

			[FieldOffset(0)]
			public uint[] AsUInt;
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
