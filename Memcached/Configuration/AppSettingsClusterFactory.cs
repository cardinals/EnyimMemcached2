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
	public class AppSettingsClusterFactory : IClusterFactory
	{
		private string sectionName;
		private DefaultClusterFactory innerConfig;

		public AppSettingsClusterFactory() : this("enyim.com/memcached/default") { }

		public AppSettingsClusterFactory(string sectionName)
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

				var config = new DefaultClusterFactory
				{
					BufferSize = section.Connection.BufferSize,
					ConnectionTimeout = section.Connection.Timeout
				};

				section.FailurePolicy.RegisterInto(config.Container);
				section.ReconnectPolicy.RegisterInto(config.Container);
				section.NodeLocator.RegisterInto(config.Container);

				config.AddNodes(section.Nodes.ToIPEndPoints());

				innerConfig = config;
			}

			return innerConfig.Create();
		}
	}
}
