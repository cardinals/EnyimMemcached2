using System;

namespace Enyim.Caching.Memcached
{
	public interface IMemcachedClientConfiguration
	{
		IOperationFactory OperationFactory { get; }
		ITranscoder Transcoder { get; }
		IPerformanceMonitor PerformanceMonitor { get; }
		IKeyTransformer KeyTransformer { get; }
	}
}
