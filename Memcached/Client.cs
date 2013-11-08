using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached
{
	class Client
	{
	}

	public class NullKeyTransformer : IKeyTransformer
	{
		public byte[] Transform(byte[] key)
		{
			return key;
		}
	}
}
