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

			section.OperationFactory.RegisterInto(innerConfig.Container);
			section.Transcoder.RegisterInto(innerConfig.Container);
			section.PerformanceMonitor.RegisterInto(innerConfig.Container);

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
	}
}
