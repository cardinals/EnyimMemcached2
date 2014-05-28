using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public class ClientTestSetupFixture : IDisposable
	{
		private const string ClusterName = "MemcachedClientTests";
		private IDisposable server;

		public ClientTestSetupFixture()
		{
			server = MemcachedServer.Run();
			new ClusterBuilder(ClusterName).FromConfiguration().Register();
			ClientConfig = new ClientConfigurationBuilder().Cluster(ClusterName).Create();
		}

		public IContainer ClientConfig { get; private set; }

		public void Dispose()
		{
			server.Dispose();
			ClientConfig.Dispose();
			ClusterManager.Shutdown(ClusterName);

			server = null;
			ClientConfig = null;
		}
	}
}
