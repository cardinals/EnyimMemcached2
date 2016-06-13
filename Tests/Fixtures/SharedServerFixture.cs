using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public abstract class SharedServerFixture
	{
		private const int Port = 11200;
		private const string ClusterName = "SharedServerTests";

		private static readonly object InitLock = new Object();

		private static int refCount;
		private static IDisposable[] servers;
		private IContainer config;

		public IContainer Config
		{
			get
			{
				InitConfig();
				return config;
			}
		}

		protected abstract void ConfigureServices(IClientBuilderServices services);

		protected void InitConfig()
		{
			lock (InitLock)
			{
				if (refCount == 0)
				{
					servers = Enumerable.Range(Port, 3).Select(p => MemcachedServer.Run(p, verbose: true)).ToArray();

					new ClusterBuilder(ClusterName)
							.Endpoints(Enumerable
											.Range(Port, 3)
											.Select(p => "localhost:" + p)
											.ToArray())
							.Register();
				}

				refCount++;
			}

			var configBuilder = new ClientConfigurationBuilder();
			ConfigureServices(configBuilder.Cluster(ClusterName).Use);

			config = configBuilder.Create();
		}

		public void Dispose()
		{
			lock (InitLock)
			{
				if (config != null)
					config.Dispose();

				if (servers != null)
				{
					refCount--;

					if (refCount == 0)
					{
						ClusterManager.Shutdown(ClusterName);

						foreach (var server in servers)
							server.Dispose();

						servers = null;
					}
				}
			}
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
