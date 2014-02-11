using System;
using System.Collections.Generic;

namespace Enyim.Caching
{
	public interface ISupportInitialize
	{
		void Initialize(IDictionary<string, string> properties);
	}
}
