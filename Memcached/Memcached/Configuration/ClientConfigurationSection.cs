using System;
using System.Collections.Generic;
using System.Configuration;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class ClientConfigurationSection : ConfigurationSection
	{
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

	}
}
