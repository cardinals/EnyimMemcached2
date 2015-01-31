using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class ClientTestSetupFixture : IDisposable
	{
		static int port = 11211;
		private const string ClusterName = "MemcachedClientTests";
		private IDisposable server;

		private string cfg;

		public ClientTestSetupFixture()
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
