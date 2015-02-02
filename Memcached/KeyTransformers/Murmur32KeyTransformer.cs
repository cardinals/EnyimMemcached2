using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching.Memcached
{
	public class Murmur32KeyTransformer : NullKeyTransformer
	{
		public override byte[] Transform(string key)
		{
			var retval = new byte[4];
			BinaryConverter.EncodeUInt32(Murmur32.ComputeHash(base.Transform(key)), retval, 0);

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
