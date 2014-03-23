using System;
using System.Configuration;
using System.Linq;
using Enyim.Caching.Configuration;
using Enyim.Reflection;

namespace Enyim.Caching.Memcached.Configuration
{
	public static class ConfigurationExtensions
	{
		public const string ClustersSectionName = "enyim.com/memcached/clusters";
		public const string DefaultClientSectionName = "enyim.com/memcached/client";

		/// <summary>
		/// Updates the ClusterBuilder from app/web.config using the cluster definition with the same name as the builder.
		/// </summary>
		public static IClusterBuilderNext FromConfiguration(this IClusterBuilder builder)
		{
			return builder.FromConfiguration(builder.Name);
		}

		/// <summary>
		/// Updates the ClusterBuilder from app/web.config using the cluster definition with the specified name.
		/// </summary>
		public static IClusterBuilderNext FromConfiguration(this IClusterBuilder builder, string name)
		{
			var section = ConfigurationManager.GetSection(ClustersSectionName) as ClustersConfigurationSection;
			if (section == null)
				throw new ConfigurationErrorsException(ClustersSectionName + " section is missing");

			var cluster = section.Clusters.ByName(name ?? String.Empty);
			var retval = builder.Endpoints(cluster.Nodes.AsIPEndPoints());

			retval
				.SocketOpts(cluster.Connection)
				.Use
					.From(cluster.FailurePolicy)
					.From(cluster.NodeLocator)
					.From(cluster.ReconnectPolicy);

			return retval;
		}

		/// <summary>
		/// Updates the ClientConfigurationBuilder from default client section in the app/web.config.
		/// </summary>
		public static IClientBuilderServices FromConfiguration(this IClientConfigurationBuilder builder)
		{
			return builder.FromConfiguration(DefaultClientSectionName);
		}

		/// <summary>
		/// Updates the ClientConfigurationBuilder from specified client section in the app/web.config.
		/// </summary>
		public static IClientBuilderServices FromConfiguration(this IClientConfigurationBuilder builder, string sectionName)
		{
			Require.NotNull(sectionName, "sectionName");

			var section = ConfigurationManager.GetSection(sectionName) as ClientConfigurationSection;
			if (section == null)
				throw new ConfigurationErrorsException(sectionName + " section is missing");

			builder
				.Cluster(section.Cluster)
				.Use
					.From(section.KeyTransformer)
					.From(section.OperationFactory)
					.From(section.Transcoder)
					.From(section.PerformanceMonitor);

			return builder.Use;
		}

		public static IClusterBuilderNext SocketOpts(this IClusterBuilderNext builder, ConnectionElement connection)
		{
			if (connection != null)
				builder.SocketOpts(
							connection.SendBufferSize,
							connection.ReceiveBufferSize,
							connection.ConnectionTimeout,
							connection.SendTimeout,
							connection.ReceiveTimeout);

			return builder;
		}

		public static ICanAddServices<TNext> From<TNext, TService>(this ICanAddServices<TNext> services, ProviderElement<TService> element)
			where TService : class
		{
			if (element != null && element.Type != null)
				services.Service(() => (TService)FastActivator.Create(element.Type));

			return services;
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
