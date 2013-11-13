using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public interface IMemcachedClientConfiguration
	{
		IOperationFactory OperationFactory { get; }
		ITranscoder Transcoder { get; }
		IPerformanceMonitor PerformanceMonitor { get; }
	}
}
