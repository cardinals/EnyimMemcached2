using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class NullPerformanceMonitor : IPerformanceMonitor
	{
		public void Get(int amount, bool success) { }
		public void Store(StoreMode mode, int amount, bool success) { }
		public void Delete(int amount, bool success) { }
		public void Mutate(MutationMode mode, int amount, bool success) { }
		public void Concatenate(ConcatenationMode mode, int amount, bool success) { }
		public void Dispose() { }
	}
}
