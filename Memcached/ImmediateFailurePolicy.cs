using System;

namespace Enyim.Caching
{
	public class ImmediateFailurePolicy : IFailurePolicy
	{
		public bool ShouldFail()
		{
			return true;
		}
	}
}
