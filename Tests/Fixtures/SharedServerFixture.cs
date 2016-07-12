using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public abstract class SharedServerFixture : IDisposable
	{
		private const string ClusterPrefix = "SharedServerTests";
		private static int InstanceCounter = 1;
		private static int Port = 11200;

		private readonly object initLock = new Object();
		private IDisposable[] servers;
		private IContainer config;
		private string clusterName;

		public IContainer Config
		{
			get
			{
				if (config == null)
				{
					InitConfig();
				}

				return config;
			}
		}

		protected abstract void ConfigureServices(IClientBuilderServices services);

		protected void InitConfig()
		{
			lock (initLock)
			{
				if (config != null) return;

				var ports = Enumerable.Range(1, 3).Select(i => Interlocked.Increment(ref Port)).ToArray();

				clusterName = ClusterPrefix + Interlocked.Increment(ref InstanceCounter);
				servers = ports.Select(p => MemcachedServer.Run(p, verbose: true)).ToArray();

				new ClusterBuilder(clusterName)
						.Endpoints(ports.Select(p => "localhost:" + p).ToArray())
						.Register();

				var configBuilder = new ClientConfigurationBuilder();
				ConfigureServices(configBuilder.Cluster(clusterName).Use);

				config = configBuilder.Create();
			}
		}

		public void Dispose()
		{
			lock (initLock)
			{
				if (config != null)
					config.Dispose();

				if (servers != null)
				{
					ClusterManager.Shutdown(clusterName);

					foreach (var server in servers)
						server.Dispose();

					servers = null;
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
