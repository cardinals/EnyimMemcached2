using System;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public static partial class MemcachedClientWithResultsExtensions
	{
		public static Task<IOperationResult> ConcateAsync(this IMemcachedClientWithResults self, ConcatenationMode mode, string key, ArraySegment<byte> data)
		{
			return self.ConcateAsync(mode, key, data, Protocol.NO_CAS);
		}

		public static Task<IOperationResult> ConcateAsync(this IMemcachedClientWithResults self, ConcatenationMode mode, string key, byte[] data, ulong cas = Protocol.NO_CAS)
		{
			return self.ConcateAsync(mode, key, new ArraySegment<byte>(data), cas);
		}

		public static IOperationResult Concate(this IMemcachedClientWithResults self, ConcatenationMode mode, string key, byte[] data, ulong cas = Protocol.NO_CAS)
		{
			return self.ConcateAsync(mode, key, new ArraySegment<byte>(data), cas).RunAndUnwrap();
		}

		public static IOperationResult Concate(this IMemcachedClientWithResults self, ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas = Protocol.NO_CAS)
		{
			return self.ConcateAsync(mode, key, data, cas).RunAndUnwrap();
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
