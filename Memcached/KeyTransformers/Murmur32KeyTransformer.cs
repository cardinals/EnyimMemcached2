using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class Murmur32KeyTransformer : IKeyTransformer
	{
		public byte[] Transform(byte[] key)
		{
			return BitConverter.GetBytes(Murmur32.ComputeHash(key));
		}
	}
}
