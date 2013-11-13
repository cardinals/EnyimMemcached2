using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching
{
	public class NullKeyTransformer : IKeyTransformer
	{
		public byte[] Transform(byte[] key)
		{
			return key;
		}
	}
}
