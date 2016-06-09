using System;
using System.Collections.Generic;
using System.Linq;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class ClientConfigurationBuilder : IClientConfigurationBuilder
	{
		private readonly Ω builder;

		public ClientConfigurationBuilder()
		{
			builder = new Ω(this);
		}

		/// <summary>
		/// Specify service overrides.
		/// </summary>
		public IClientBuilderServices Use { get { return builder; } }

		/// <summary>
		/// Creates the container that can be used to initialize the MemcachedClient.
		/// </summary>
		/// <returns></returns>
		public IContainer Create()
		{
			return builder.Create();
		}

		/// <summary>
		/// Specifies the cluster to be used by the clients using this configuration.
		/// </summary>
		/// <param name="name">The name of the cluster.</param>
		/// <returns></returns>
		public IClientConfigurationBuilderDefaults Cluster(string name)
		{
			return builder.Cluster(name);
		}

		#region [ Builder                      ]

		private class Ω : IClientBuilderServicesNext
		{
			private ClientConfigurationBuilder owner;
			private Container container;
			private string clusterName;
			private bool isReadOnly;
			private FunqContainerWrapper wrapper;

			public Ω(ClientConfigurationBuilder owner)
			{
				this.owner = owner;
			}

			public IClientBuilderServices Use { get { return this; } }

			public IClientConfigurationBuilder Cluster(string name)
			{
				name = name ?? String.Empty;

				if (name != clusterName)
				{
					clusterName = name;
					InitContainer(true);
				}

				return owner;
			}

			public IContainer Create()
			{
				InitContainer(false);

				try { return wrapper; }
				finally
				{
					isReadOnly = true;
					container = null;
					owner = null;
				}
			}

			public IClientBuilderServicesNext Service<TService>(Func<IContainer, TService> factory)
			{
				InitContainer(false);

				container.Register(_ => factory(wrapper));

				return this;
			}

			public IClientBuilderServicesNext Service<TService>(Type implementation, Action<TService> initializer = null)
				where TService : class
			{
				InitContainer(false);

				var reg = container.AutoWireAs<TService>(implementation);
				if (initializer != null) reg.InitializedBy((_, i) => initializer(i));

				return this;
			}

			private void InitContainer(bool recreate)
			{
				ThrowIfReadOnly();

				if (container != null)
				{
					if (!recreate) return;

					container.Dispose();
				}

				var cluster = ClusterManager.GetCluster(clusterName);
				container = cluster.CreateChildContainer();
				container.AddClientDefauls();
				wrapper = new FunqContainerWrapper(container);
			}

			private void ThrowIfReadOnly()
			{
				if (isReadOnly) throw new InvalidOperationException("Client config has been created already and cannot be changed.");
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
