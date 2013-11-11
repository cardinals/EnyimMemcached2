using System;
using System.Collections.Generic;
using System.Configuration;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class MemcachedClientConfiguration : IMemcachedClientConfiguration
	{
		private Funq.Container container;
		private IOperationFactory operationFactory;
		private ITranscoder transcoder;
		private IKeyTransformer keyTransformer;

		public MemcachedClientConfiguration()
		{
			container = new Funq.Container();

			container.Register<IOperationFactory>(c => new MemcachedOperationFactory());
			container.Register<ITranscoder>(c => new DefaultTranscoder());
			container.Register<IKeyTransformer>(c => new Murmur32KeyTransformer());
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

		IKeyTransformer IMemcachedClientConfiguration.KeyTransformer
		{
			get { return keyTransformer ?? (keyTransformer = container.Resolve<IKeyTransformer>()); }
		}
	}
}
