﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Enyim.Caching.Memcached
{
	public static class Protocol
	{
		public const byte RequestMagic = 0x80;
		public const byte ResponseMagic = 0x81;
		public const int HeaderLength = 24;

		public const int MaxKeyLength = 0xffff;
		public const int MaxExtraLength = 0xff;

		internal const int HEADER_INDEX_MAGIC = 0;
		internal const int HEADER_INDEX_OPCODE = 1;
		internal const int HEADER_INDEX_KEY = 2; // 2-3
		internal const int HEADER_INDEX_EXTRA = 4;
		internal const int HEADER_INDEX_DATATYPE = 5;
		internal const int HEADER_INDEX_STATUS = 6; // 6-7
		internal const int HEADER_INDEX_BODY_LENGTH = 8; // 8-11
		internal const int HEADER_INDEX_OPAQUE = 12; // 12-15
		internal const int HEADER_INDEX_CAS = 16; // 16-23

		public static class Status
		{
			public const int Success = 0;
		}

		public const ulong NO_CAS = 0;
		public const ulong MUTATE_DEFAULT_DELTA = 1;
		public const ulong MUTATE_DEFAULT_VALUE = 1;

		private static readonly bool[] SilentOps;

		static Protocol()
		{
			var values = Enum.GetValues(typeof(OpCode)).Cast<OpCode>().ToDictionary(o => o.ToString(), o => (int)o);
			var flags = new bool[values.Values.Max() + 1];

			foreach (var kvp in values)
			{
				if (kvp.Key.EndsWith("Q"))
					flags[kvp.Value] = true;
			}

			SilentOps = flags;
		}

		internal static bool IsSilent(byte opcode)
		{
			return SilentOps[opcode];
		}

		internal static bool IsSilent(OpCode opcode)
		{
			return SilentOps[(int)opcode];
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
