using System;
using System.Linq;
using System.Configuration;
using Enyim.Caching.Configuration;

namespace Enyim.Caching.Memcached.Configuration
{
	public class ClientConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("cluster")]
		public string Cluster
		{
			get { return (string)base["cluster"]; }
			set { base["cluster"] = value; }
		}

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

		[ConfigurationProperty("keyTransformer")]
		public ProviderElement<IKeyTransformer> KeyTransformer
		{
			get { return (ProviderElement<IKeyTransformer>)base["keyTransformer"]; }
			set { base["keyTransformer"] = value; }
		}
	}
}
