using System;
using System.Linq;
using System.Configuration;
using Funq;
using System.Collections.Generic;

namespace Enyim.Caching.Configuration
{
	public class ClustersConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty("", IsDefaultCollection = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
		public ClusterConfigurationCollection Clusters
		{
			get { return (ClusterConfigurationCollection)base[""]; }
			set { base[""] = value; }
		}
	}
}
