using System;

namespace Enyim.Caching.Memcached
{
	public class ImmediateFailurePolicy : INodeFailurePolicy
	{
		public bool ShouldFail()
		{
			return true;
		}
	}
}
