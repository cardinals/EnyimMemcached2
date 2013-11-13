using System;
using System.Collections.Generic;
using System.Configuration;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class ClientConfigurationSection : ConfigurationSection, IMemcachedClientConfiguration
	{
		private IOperationFactory operationFactory;
		private ITranscoder transcoder;
		private IPerformanceMonitor performanceMonitor;

		[ConfigurationProperty("operationFactory")]
		public ProviderElement<IOperationFactory> OperationFactory
		{
			get { return (ProviderElement<IOperationFactory>)base["operationFactory"]; }
			set { base["operationFactory"] = value; }
		}

		[ConfigurationProperty("transcoder")]
		public ProviderElement<ITranscoder> Transcoder
		{
			get { return (ProviderElement<ITranscoder>)base["transcoder"]; }
			set { base["transcoder"] = value; }
		}

		[ConfigurationProperty("performanceMonitor")]
		public ProviderElement<IPerformanceMonitor> PerformanceMonitor
		{
			get { return (ProviderElement<IPerformanceMonitor>)base["performanceMonitor"]; }
			set { base["performanceMonitor"] = value; }
		}

		IOperationFactory IMemcachedClientConfiguration.OperationFactory
		{
			get { return innerConfig.OperationFactory; }
		}

		ITranscoder IMemcachedClientConfiguration.Transcoder
		{
			get { return innerConfig.Transcoder; }
		}

		IPerformanceMonitor IMemcachedClientConfiguration.PerformanceMonitor
		{
			get { return innerConfig.PerformanceMonitor; }
		}

		private IMemcachedClientConfiguration innerConfig;

		protected override void PostDeserialize()
		{
			base.PostDeserialize();

			var innerConfig = new ClientConfiguration();

			RegisterProviderElement(innerConfig, OperationFactory);
			RegisterProviderElement(innerConfig, Transcoder);
			RegisterProviderElement(innerConfig, PerformanceMonitor);

			this.innerConfig = innerConfig;
		}

		static T Create<T>(ProviderElement<T> element) where T : class
		{
			var type = element.Type;
			if (type == null) return null;

			var instance = (T)Enyim.Reflection.FastActivator.Create(element.Type);
			var init = instance as ISupportInitialize;

			if (init != null)
				init.Initialize(element.Parameters);

			return instance;
		}

		private static IRegistration<TContract> RegisterProviderElement<TContract>(ClientConfiguration config, ProviderElement<TContract> element)
	where TContract : class
		{
			if (element == null) return null;

			var type = element.Type;
			if (type == null) return null;

			var reg = config.Container.AutoWireAs<TContract>(type);

			if (typeof(ISupportInitialize).IsAssignableFrom(type))
				reg.InitializedBy((c, instance) => ((ISupportInitialize)instance).Initialize(element.Parameters));

			return reg;
		}

	}
}
