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
