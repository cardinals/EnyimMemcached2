using System;
using System.Collections.Generic;
using System.Linq;

namespace Enyim.Caching.Memcached.Operations
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
		internal const int HEADER_INDEX_STATUS = 6;	// 6-7
		internal const int HEADER_INDEX_BODY = 8; // 8-11
		internal const int HEADER_INDEX_OPAQUE = 12; // 12-15
		internal const int HEADER_INDEX_CAS = 16; // 16-23

		internal const int MUTATE_EXTRA_LENGTH = 20;

		internal const byte SILENT_MASK = 0x10;
		internal const byte LOUD_MASK = 0x0F;

		public static class Status
		{
			public const int Success = 0;
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
