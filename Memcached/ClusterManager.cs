using System;
using Funq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;

namespace Enyim.Caching.Memcached
{
	public static class ClusterManager
	{
		private static readonly ConcurrentDictionary<string, ICluster> clusters = new ConcurrentDictionary<string, ICluster>();

		public static ICluster Get(string name)
		{
			ICluster retval;

			if (!clusters.TryGetValue(name, out retval))
				throw new InvalidOperationException("cluster is not registered: " + (name ?? "<default cluster>"));

			return retval;
		}

		public static ICluster Register(IClusterFactory factory)
		{
			return Register(null, factory);
		}

		public static ICluster Register(string name, IClusterFactory factory)
		{
			var retval = clusters.AddOrUpdate(name,
												_ => factory.Create(),
												(a, b) =>
												{
													throw new InvalidOperationException("cluster already exists: " + (name ?? "<default cluster>"));
												});

			return retval;
		}

		public static ICluster RegisterConfigurationSection(string sectionName)
		{
			var section = ConfigurationManager.GetSection(sectionName) as ClusterConfigurationSection;
			if (section == null)
				throw new ConfigurationErrorsException(String.Format("Section {0} was not found or it's not a ClusterConfigurationSection", sectionName));

			var config = new BasicConfiguration
			{
				BufferSize = section.Connection.BufferSize,
				ConnectionTimeout = section.Connection.Timeout
			};

			RegisterProviderElement(config, section.FailurePolicy, ReuseScope.None);
			RegisterProviderElement(config, section.ReconnectPolicy);
			RegisterProviderElement(config, section.NodeLocator);
			RegisterProviderElement(config, section.KeyTransformer);

			config.AddNodes(section.Nodes.ToIPEndPoints());

			return Register(sectionName, config);
		}

		private static IRegistration<TContract> RegisterProviderElement<TContract>(BasicConfiguration config, ProviderElement<TContract> element, ReuseScope scope = ReuseScope.Default)
			where TContract : class
		{
			if (element == null) return null;

			var type = element.Type;
			if (type == null) return null;

			var reg = config.Container.Register<TContract>(type);

			if (typeof(ISupportInitialize).IsAssignableFrom(type))
				reg.InitializedBy((c, instance) => ((ISupportInitialize)instance).Initialize(element.Parameters));

			if (scope != ReuseScope.Default)
				reg.ReusedWithin(scope);

			return reg;
		}
	}
}

#region [ License information          ]

/* ************************************************************
 *
 *    Copyright (c) Attila Kiskó, enyim.com
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion
