using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class ClusterBuilder : IClusterBuilder, IFluentSyntax
	{
		private readonly Ω builder;

		public ClusterBuilder() : this(String.Empty) { }

		public ClusterBuilder(string name)
		{
			Require.NotNull(name, "name");

			Name = name;
			builder = new Ω(this);
		}

		/// <summary>
		/// Gets the name of the current builder.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Specify the nodes of the clusters.
		/// </summary>
		/// <param name="endpoints">List of endpoint addresses.</param>
		/// <returns></returns>
		public IClusterBuilderNext Endpoints(IEnumerable<IPEndPoint> endpoints)
		{
			return builder.Endpoints(endpoints);
		}

		/// <summary>
		/// Registers the cluster configuration.
		/// </summary>
		public IContainer Register()
		{
			return builder.Register();
		}

		#region [ Builder                      ]

		private class Ω : IClusterBuilderServicesNext, IClusterBuilderNext
		{
			private ClusterBuilder owner;
			private Funq.Container container;

			public Ω(ClusterBuilder owner)
			{
				this.owner = owner;
				this.container = new Funq.Container();
				this.container.AddClusterDefauls();
			}

			public IClusterBuilderServices Use { get { return this; } }

			public IClusterBuilderNext Endpoints(IEnumerable<IPEndPoint> endpoints)
			{
				Require.NotNull(endpoints, "endpoints");
				ThrowIfReadOnly();

				var tmp = endpoints.ToArray();
				if (tmp.Length == 0) throw new ArgumentException("Endpoints must be specified.");

				container.RegisterCluster(tmp);

				return this;
			}

			public IContainer Register()
			{
				ThrowIfReadOnly();

				try
				{
					ClusterManager.CacheCluster(owner.Name, container);

					return new FunqContainerWrapper(container);
				}
				finally
				{
					container = null;
					owner = null;
				}
			}

			public IClusterBuilderServicesNext Service<TService>(Func<TService> factory)
			{
				ThrowIfReadOnly();

				container.Register<TService>(_ => factory());

				return this;
			}

			public IClusterBuilderServicesNext Service<TService>(Type implementation, Action<TService> initializer) where TService : class
			{
				ThrowIfReadOnly();

				var reg = container.AutoWireAs<TService>(implementation);
				if (initializer != null) reg.InitializedBy((_, i) => initializer(i));

				return this;
			}

			private void ThrowIfReadOnly()
			{
				if (container == null) throw new InvalidOperationException("Cluster is already registered and cannot be reconfigured.");
			}
		}

		#endregion
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
