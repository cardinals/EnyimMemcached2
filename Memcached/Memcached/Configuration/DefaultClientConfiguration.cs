using System;
using System.Collections.Generic;
using System.Configuration;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class DefaultClientConfiguration : IMemcachedClientConfiguration
	{
		private readonly Funq.Container container;

		public DefaultClientConfiguration()
		{
			container = new Funq.Container();

			container.Register<IOperationFactory>(c => new MemcachedOperationFactory());
			container.Register<ITranscoder>(c => new DefaultTranscoder());
			container.AutoWireAs<IPerformanceMonitor, NullPerformanceMonitor>();
		}

		public Container Container { get { return container; } }

		public IOperationFactory OperationFactory
		{
			get { return container.Resolve<IOperationFactory>(); }
		}

		public ITranscoder Transcoder
		{
			get { return container.Resolve<ITranscoder>(); }
		}

		public IPerformanceMonitor PerformanceMonitor
		{
			get { return container.Resolve<IPerformanceMonitor>(); }
		}
	}
}
