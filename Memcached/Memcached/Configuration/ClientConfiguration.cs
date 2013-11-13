using System;
using System.Collections.Generic;
using System.Configuration;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class ClientConfiguration : IMemcachedClientConfiguration
	{
		private Funq.Container container;
		private IOperationFactory operationFactory;
		private ITranscoder transcoder;
		private IPerformanceMonitor performanceMonitor;

		public ClientConfiguration()
		{
			container = new Funq.Container();

			container.Register<IOperationFactory>(c => new MemcachedOperationFactory());
			container.Register<ITranscoder>(c => new DefaultTranscoder());
			container.AutoWireAs<IPerformanceMonitor, NullPerformanceMonitor>();
		}

		public Container Container { get { return container; } }

		IOperationFactory IMemcachedClientConfiguration.OperationFactory
		{
			get { return operationFactory ?? (operationFactory = container.Resolve<IOperationFactory>()); }
		}

		ITranscoder IMemcachedClientConfiguration.Transcoder
		{
			get { return transcoder ?? (transcoder = container.Resolve<ITranscoder>()); }
		}

		IPerformanceMonitor IMemcachedClientConfiguration.PerformanceMonitor
		{
			get { return performanceMonitor ?? (performanceMonitor = container.Resolve<IPerformanceMonitor>()); }
		}
	}
}
