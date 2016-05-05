using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public class PrivateServerFixture : IDisposable
	{
		static int port = 11211;
		private const string ClusterName = "MemcachedClientTests";
		private IDisposable server;

		private string cfg;

		static PrivateServerFixture()
		{
			Enyim.Caching.NLogFactory.Use();
		}

		public PrivateServerFixture()
		{
			var p = Interlocked.Increment(ref port);
			server = MemcachedServer.Run(p);

			cfg = ClusterName + p;
			new ClusterBuilder(cfg).Endpoints("localhost:" + p).Register();
			ClientConfig = new ClientConfigurationBuilder().Cluster(cfg).Create();
		}

		public IContainer ClientConfig { get; private set; }

		public void Dispose()
		{
			server.Dispose();
			ClientConfig.Dispose();
			ClusterManager.Shutdown(cfg);

			server = null;
			ClientConfig = null;
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
