﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public abstract class PrivateServerFixture
	{
		private static int Port = 11211;
		private const string ClusterName = "MemcachedClientTests";

		private object InitLock = new Object();

		private IDisposable server;
		private string clusterName;
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
				if (clusterName == null)
				{
					var p = Interlocked.Increment(ref Port);
					server = MemcachedServer.Run(p);

					clusterName = ClusterName + p;
					new ClusterBuilder(clusterName).Endpoints("localhost:" + p).Register();

					var clientBuilder = new ClientConfigurationBuilder();
					ConfigureServices(clientBuilder.Cluster(clusterName).Use);

					config = clientBuilder.Create();
				}
			}
		}

		public void Dispose()
		{
			if (server != null)
			{
				server.Dispose();
				server = null;
			}

			if (config != null)
			{
				config.Dispose();
				config = null;
			}

			ClusterManager.Shutdown(clusterName);
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
