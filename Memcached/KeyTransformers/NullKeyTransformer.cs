using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class NullKeyTransformer : IKeyTransformer
	{
		public virtual byte[] Transform(string key)
		{
			return Encoding.UTF8.GetBytes(key);
		}
	}
}
