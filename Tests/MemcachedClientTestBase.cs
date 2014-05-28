using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Tests
{
	public abstract class MemcachedClientTestBase
	{
		private readonly string name;
		private readonly Random random;

		protected MemcachedClientTestBase(string name)
		{
			this.name = name + "_";
			this.random = new Random();
		}

		protected string GetUniqueKey(string prefix = null)
		{
			return (!String.IsNullOrEmpty(prefix) ? prefix + "_" : "") + name + DateTime.Now.Ticks;
		}

		protected string[] GetUniqueKeys(string prefix = null, int max = 5)
		{
			return Enumerable
					.Range(0, max)
					.Select(i => GetUniqueKey(prefix) + "_" + i)
					.ToArray();
		}

		protected string GetRandomString()
		{
			return "unit_test_value_" + random.Next();
		}
	}
}
