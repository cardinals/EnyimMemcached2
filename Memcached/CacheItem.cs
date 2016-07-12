using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Enyim.Caching.Memcached
{
	public struct CacheItem : IDisposable
	{
		public static readonly CacheItem Empty = new CacheItem(0, ByteBuffer.Empty);

		private readonly uint flags;
		private readonly ByteBuffer data;

		public CacheItem(uint flags, ByteBuffer data)
		{
			this.flags = flags;
			this.data = data;
		}

		public uint Flags { get { return flags; } }
		public ByteBuffer Data { get { return data; } }

		public void Dispose()
		{
			data.Dispose();
		}

		public override bool Equals(object obj)
		{
			return obj is CacheItem && Equals((CacheItem)obj);
		}

		public bool Equals(CacheItem obj)
		{
			return obj.flags == flags && obj.data.Equals(data);
		}

		public override int GetHashCode()
		{
			return HashCodeCombiner.Combine(flags.GetHashCode(), data.GetHashCode());
		}

		public static bool operator ==(CacheItem a, CacheItem b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(CacheItem a, CacheItem b)
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
