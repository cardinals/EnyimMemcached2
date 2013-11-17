using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching
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
