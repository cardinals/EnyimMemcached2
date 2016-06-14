using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Enyim.Caching.Memcached
{
	//-----------------------------------------------------------------------------
	// MurmurHash3 was written by Austin Appleby, and is placed in the public
	// domain. The author hereby disclaims copyright to this source code.

	//
	// this is exactly the same algorithm as MurmurHash3_x64_128 except
	// we do not need all 128 bits, only 64
	internal static class Murmur64_64
	{
		public unsafe static ulong ComputeHash(byte[] data, int length, uint seed = 0)
		{
			var nblocks = length / 16;

			ulong h1 = seed;
			ulong h2 = seed;

			const ulong c1 = 0x87c37b91114253d5L;
			const ulong c2 = 0x4cf5ad432745937fL;

			//----------
			// body
			ulong k1 = 0;
			ulong k2 = 0;

			fixed (byte* ptr = data)
			{
				ulong* current = (ulong*)ptr;

				while (nblocks-- > 0)
				{
					k1 = *current++;
					k2 = *current++;

					k1 *= c1; k1 = RotL64(k1, 31); k1 *= c2; h1 ^= k1;

					h1 = RotL64(h1, 27); h1 += h2; h1 = h1 * 5 + 0x52dce729L;

					k2 *= c2; k2 = RotL64(k2, 33); k2 *= c1; h2 ^= k2;

					h2 = RotL64(h2, 31); h2 += h1; h2 = h2 * 5 + 0x38495ab5L;
				}

				//----------
				// tail

				var tail = (byte*)current;

				k1 = 0;
				k2 = 0;

				switch (length & 15)
				{
					case 15: k2 ^= (ulong)(tail[14]) << 48; goto case 14;
					case 14: k2 ^= (ulong)(tail[13]) << 40; goto case 13;
					case 13: k2 ^= (ulong)(tail[12]) << 32; goto case 12;
					case 12: k2 ^= (ulong)(tail[11]) << 24; goto case 11;
					case 11: k2 ^= (ulong)(tail[10]) << 16; goto case 10;
					case 10: k2 ^= (ulong)(tail[9]) << 8; goto case 9;
					case 9:
						k2 ^= (ulong)(tail[8]) << 0;
						k2 *= c2; k2 = RotL64(k2, 33); k2 *= c1; h2 ^= k2;
						goto case 8;
					case 8: k1 ^= (ulong)(tail[7]) << 56; goto case 7;
					case 7: k1 ^= (ulong)(tail[6]) << 48; goto case 6;
					case 6: k1 ^= (ulong)(tail[5]) << 40; goto case 5;
					case 5: k1 ^= (ulong)(tail[4]) << 32; goto case 4;
					case 4: k1 ^= (ulong)(tail[3]) << 24; goto case 3;
					case 3: k1 ^= (ulong)(tail[2]) << 16; goto case 2;
					case 2: k1 ^= (ulong)(tail[1]) << 8; goto case 1;
					case 1:
						k1 ^= (ulong)(tail[0]) << 0;
						k1 *= c1; k1 = RotL64(k1, 31); k1 *= c2; h1 ^= k1;
						break;
				}
			}

			//----------
			// finalization

			h1 ^= (ulong)length; h2 ^= (ulong)length;

			h1 += h2;
			h2 += h1;

			//h1 = FMix64(h1);
			h1 ^= h1 >> 33;
			h1 *= 0xff51afd7ed558ccd;
			h1 ^= h1 >> 33;
			h1 *= 0xc4ceb9fe1a85ec53;
			h1 ^= h1 >> 33;

			//h2 = FMix64(h2);
			h2 ^= h2 >> 33;
			h2 *= 0xff51afd7ed558ccd;
			h2 ^= h2 >> 33;
			h2 *= 0xc4ceb9fe1a85ec53;
			h2 ^= h2 >> 33;

			h1 += h2;
			h2 += h1;

			return h1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static ulong RotL64(ulong x, int r)
		{
			return (x << r) | (x >> (64 - r));
		}

		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//private static ulong FMix64(ulong k)
		//{
		//	k ^= k >> 33;
		//	k *= 0xff51afd7ed558ccd;
		//	k ^= k >> 33;
		//	k *= 0xc4ceb9fe1a85ec53;
		//	k ^= k >> 33;

		//	return k;
		//}
	}
}
