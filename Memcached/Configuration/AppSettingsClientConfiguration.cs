using System;
using System.Collections.Generic;
using System.Configuration;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class AppSettingsClientConfiguration : IMemcachedClientConfiguration
	{
		public const string DefaultSection = "enyim.com/memcached/client";

		private readonly IMemcachedClientConfiguration innerConfig;

		public AppSettingsClientConfiguration()
			: this(DefaultSection) { }

		public AppSettingsClientConfiguration(string section)
			: this(ConfigurationManager.GetSection(section) as ClientConfigurationSection) { }

		public AppSettingsClientConfiguration(ClientConfigurationSection section)
		{
			Require.NotNull(section, "section");

			var innerConfig = new DefaultClientConfiguration();

			section.OperationFactory.TryRegisterInto(innerConfig.Container);
			section.Transcoder.TryRegisterInto(innerConfig.Container);
			section.PerformanceMonitor.TryRegisterInto(innerConfig.Container);
			section.KeyTransformer.TryRegisterInto(innerConfig.Container);

			this.innerConfig = innerConfig;
		}

		public IOperationFactory OperationFactory
		{
			get { return innerConfig.OperationFactory; }
		}

		public ITranscoder Transcoder
		{
			get { return innerConfig.Transcoder; }
		}

		public IPerformanceMonitor PerformanceMonitor
		{
			get { return innerConfig.PerformanceMonitor; }
		}

		public IKeyTransformer KeyTransformer
		{
			get { return innerConfig.KeyTransformer; }
		}
	}
}
