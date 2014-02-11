using System;
using System.Collections.Generic;
using System.Diagnostics;
using Enyim.Caching.Configuration;
using Funq;
using CM = System.Configuration.ConfigurationManager;

namespace Enyim.Caching.Memcached.Configuration
{
	public static class ConfigurationManager
	{
		private static readonly Dictionary<string, Container> ClientCache = new Dictionary<string, Container>();
		private static readonly Dictionary<string, Container> ClusterCache = new Dictionary<string, Container>();
		private static readonly object OnClientCache = new Object();
		private static readonly object OnClusterCache = new Object();

		public static IContainer GetSection(string section)
		{
			return new ContainerWrapper(GetClientContainer(section));
		}

		private static Container GetClientContainer(string clientName)
		{
			Container retval;

			if (!ClientCache.TryGetValue(clientName, out retval))
			{
				lock (OnClientCache)
				{
					if (!ClientCache.TryGetValue(clientName, out retval))
					{
						retval = BuildClientContainer(clientName);
						ClientCache[clientName] = retval;
					}
				}
			}

			return retval;
		}

		private static Container BuildClientContainer(string clientName)
		{
			var section = CM.GetSection(clientName) as ClientConfigurationSection;
			var root = GetClusterContainer(section.Cluster ?? String.Empty);
			var retval = root.CreateChildContainer();

			section.RegisterInto(retval);

			return retval;
		}

		private static Container GetClusterContainer(string clusterName)
		{
			Debug.Assert(clusterName != null);
			Container retval;

			if (!ClusterCache.TryGetValue(clusterName, out retval))
			{
				lock (OnClusterCache)
				{
					if (!ClusterCache.TryGetValue(clusterName, out retval))
					{
						retval = BuildClusterContainer(clusterName);
						ClusterCache[clusterName] = retval;
					}
				}
			}

			return retval;
		}

		private static Container BuildClusterContainer(string clusterName)
		{
			var section = CM.GetSection("enyim.com/memcached/clusters") as ClustersConfigurationSection;
			Require.NotNull(section, "name", "enyim.com/memcached/clusters section is missing");

			var retval = new Container();
			section.Clusters.ByName(clusterName).RegisterInto(retval);

			return retval;
		}

		#region [ ContainerWrapper             ]

		private class ContainerWrapper : IContainer
		{
			private readonly Container root;

			public ContainerWrapper(Container root)
			{
				this.root = root;
			}

			public TService Resolve<TService>()
			{
				return root.Resolve<TService>();
			}
		}

		#endregion
	}
}