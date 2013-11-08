using System;
using System.Collections.Generic;

namespace Enyim.Caching.Memcached
{
	public interface ISupportInitialize
	{
		void Initialize(Dictionary<string, string> properties);
	}
}
