using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class AppSettingsConfiguration : IClusterFactory
	{
		private string sectionName;
		private BasicConfiguration innerConfig;

		public AppSettingsConfiguration() : this("enyim.com/memcached/default") { }

		public AppSettingsConfiguration(string sectionName)
		{
			this.sectionName = sectionName;
		}

		public ICluster Create()
		{
			if (innerConfig == null)
			{
				var section = ConfigurationManager.GetSection(sectionName) as ClusterConfigurationSection;
				if (section == null)
					throw new ConfigurationErrorsException(String.Format("Section {0} was not found or it's not a ClusterConfigurationSection", sectionName));

				var config = new BasicConfiguration
				{
					BufferSize = section.Connection.BufferSize,
					ConnectionTimeout = section.Connection.Timeout
				};

				RegisterProviderElement(config, section.FailurePolicy);
				RegisterProviderElement(config, section.ReconnectPolicy);
				RegisterProviderElement(config, section.NodeLocator);
				RegisterProviderElement(config, section.KeyTransformer);

				config.AddNodes(section.Nodes.ToIPEndPoints());

				innerConfig = config;
			}

			return innerConfig.Create();
		}

		private static IRegistration<TContract> RegisterProviderElement<TContract>(BasicConfiguration config, ProviderElement<TContract> element, ReuseScope scope = ReuseScope.Default)
			where TContract : class
		{
			if (element == null) return null;

			var type = element.Type;
			if (type == null) return null;

			var reg = config.Container.AutoWireAs<TContract>(type);

			if (typeof(ISupportInitialize).IsAssignableFrom(type))
				reg.InitializedBy((c, instance) => ((ISupportInitialize)instance).Initialize(element.Parameters));

			if (scope != ReuseScope.Default)
				reg.ReusedWithin(scope);

			return reg;
		}
	}
}
