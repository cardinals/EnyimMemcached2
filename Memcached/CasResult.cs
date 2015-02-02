using System;
using System.Collections.Generic;
using System.Linq;

namespace Enyim.Caching.Memcached
{
	public struct CasResult<T>
	{
		private readonly T result;
		private readonly ulong cas;
		private readonly int statusCode;

		public CasResult(T result, ulong cas, int statusCode)
		{
			this.result = result;
			this.cas = cas;
			this.statusCode = statusCode;
		}

		public T Result { get { return result; } }
		public ulong Cas { get { return cas; } }
		public int StatusCode { get { return statusCode; } }
	}
}
