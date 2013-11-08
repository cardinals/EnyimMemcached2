using System;

namespace Enyim.Caching.Memcached
{
	public class ImmediateFailurePolicy : IFailurePolicy
	{
		public bool ShouldFail()
		{
			return true;
		}
	}
}
