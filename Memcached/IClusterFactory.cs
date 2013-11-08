using System;

namespace Enyim.Caching.Memcached
{
	public interface IClusterFactory
	{
		ICluster Create();
	}
}
