using System;
using System.Linq;

namespace Enyim.Caching.Memcached.Configuration
{
	public class ClientConfigurationBuilder : IClientConfigurationBuilder
	{
		private readonly Ω builder;

		public ClientConfigurationBuilder()
		{
			builder = new Ω(this);
		}

		public IClientBuilderServices Add { get { return builder; } }

		public IContainer Create()
		{
			return builder.Create();
		}

		public IClientConfigurationBuilderDefaults Cluster(string name)
		{
			return builder.Cluster(name);
		}

		public void MakeDefault(bool force = false)
		{
			builder.MakeDefault(force);
		}

		#region [ Builder                      ]

		private class Ω : IClientBuilderServicesNext
		{
			private ClientConfigurationBuilder owner;
			private Funq.Container container;
			private string clusterName;
			private bool isReadOnly;

			public Ω(ClientConfigurationBuilder owner)
			{
				this.owner = owner;
			}

			public IClientBuilderServices Add { get { return this; } }

			private void InitCluster()
			{
				ThrowIfReadOnly();

				var cluster = ConfigurationManager.GetCluster(clusterName);

				if (container != null) container.Dispose();
				container = cluster.CreateChildContainer();
				container.AddClientDefauls();
			}

			public IClientConfigurationBuilder Cluster(string name)
			{
				name = name ?? String.Empty;

				if (name != clusterName)
				{
					clusterName = name;
					InitCluster();
				}

				return owner;
			}

			public IContainer Create()
			{
				InitCluster();

				try { return new FunqContainerWrapper(container); }
				finally
				{
					isReadOnly = true;
					container = null;
					owner = null;
				}
			}

			public IClientBuilderServicesNext Service<TService>(Func<TService> factory)
			{
				InitCluster();

				container.Register<TService>(_ => factory());

				return this;
			}

			private void ThrowIfReadOnly()
			{
				if (isReadOnly) throw new InvalidOperationException("Client cannot be reconfigured.");
			}

			public void MakeDefault(bool force = false)
			{
				if (MemcachedClientBase.DefaultContainer != null)
				{
					if (!force)
						throw new InvalidOperationException("There is already a default configuration defined for MemcachedClient");

					MemcachedClientBase.DefaultContainer.Dispose();
				}

				MemcachedClientBase.DefaultContainer = Create();
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
