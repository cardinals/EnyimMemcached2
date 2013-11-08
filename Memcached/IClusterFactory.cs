using System;

namespace Enyim.Caching
{
	public interface IClusterFactory
	{
		ICluster Create();
	}
}
