using System;
using System.Configuration;
using System.Linq;
using Enyim.Caching.Configuration;
using Enyim.Reflection;

namespace Enyim.Caching.Memcached.Configuration
{
	using CM = System.Configuration.ConfigurationManager;

	public static class ConfigurationExtensions
	{
		/// <summary>
		/// Updates the ClusterBuilder from the clusters section in the app/web.config using the Name of the builder.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IClusterBuilder FromConfiguration(this IClusterBuilder builder)
		{
			return builder.FromConfiguration(builder.Name);
		}

		/// <summary>
		/// Updates the ClusterBuilder from the clusters section in the app/web.config using the specified name.
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IClusterBuilder FromConfiguration(this IClusterBuilder builder, string name)
		{
			Require.NotNull(name, "name");

			var section = CM.GetSection("enyim.com/memcached/clusters") as ClustersConfigurationSection;
			if (section == null)
				throw new ConfigurationErrorsException("enyim.com/memcached/clusters section is missing");

			var cluster = section.Clusters.ByName(name);

			builder
				.Endpoints(cluster.Nodes.AsIPEndPoints())
				.SocketOpts(cluster.Connection)
				.Add
					.From(cluster.FailurePolicy)
					.From(cluster.NodeLocator)
					.From(cluster.ReconnectPolicy);

			return builder;
		}

		private static IClusterBuilderNext SocketOpts(this IClusterBuilderNext builder, ConnectionElement connection)
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

		public static IClientConfigurationBuilder FromConfiguration(this IClientConfigurationBuilder builder, string sectionName)
		{
			Require.NotNull(sectionName, "section");

			var section = CM.GetSection("enyim.com/memcached/clients") as ClientConfigurationSection;
			if (section == null)
				throw new ConfigurationErrorsException(sectionName + " section is missing");

			builder
				.Cluster(section.Cluster)
				.Add
					.From(section.KeyTransformer)
					.From(section.OperationFactory)
					.From(section.Transcoder)
					.From(section.PerformanceMonitor);

			return builder;
		}

		private static ICanAddServices<TNext> From<TNext, TService>(this ICanAddServices<TNext> services, ProviderElement<TService> element)
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
