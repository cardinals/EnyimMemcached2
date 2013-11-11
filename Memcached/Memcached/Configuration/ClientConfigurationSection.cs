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
		private IKeyTransformer keyTransformer;

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

		[ConfigurationProperty("keyTransformer")]
		public ProviderElement<IKeyTransformer> KeyTransformer
		{
			get { return (ProviderElement<IKeyTransformer>)base["keyTransformer"]; }
			set { base["keyTransformer"] = value; }
		}

		IOperationFactory IMemcachedClientConfiguration.OperationFactory
		{
			get { return operationFactory ?? (operationFactory = Create(OperationFactory)); }
		}

		ITranscoder IMemcachedClientConfiguration.Transcoder
		{
			get { return transcoder ?? (transcoder = Create(Transcoder)); }
		}

		IKeyTransformer IMemcachedClientConfiguration.KeyTransformer
		{
			get { return keyTransformer ?? (keyTransformer = Create(KeyTransformer)); }
		}

		static T Create<T>(ProviderElement<T> element) where T : class
		{
			var instance = (T)Enyim.Reflection.FastActivator.Create(element.Type);
			var isi = instance as ISupportInitialize;

			if (isi != null)
				isi.Initialize(element.Parameters);

			return instance;
		}
	}
}
