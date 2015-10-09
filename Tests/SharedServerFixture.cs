using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public class SharedServerFixture : IDisposable
	{
		private const int Port = 11211;
		private const string ClusterName = "SharedServerTests";

		private static readonly object InitLock = new Object();
		private static IContainer Config;
		private static int refCount;
		private static IDisposable server;

		private static IContainer GetConfig()
		{
			lock (InitLock)
			{
				if (Config == null)
				{
					server = MemcachedServer.Run(Port);

					new ClusterBuilder(ClusterName).Endpoints("localhost:" + Port).Register();
					Config = new ClientConfigurationBuilder().Cluster(ClusterName).Create();
				}

				refCount++;
			}

			return Config;
		}

		private void ReleaseConfig()
		{
			lock (InitLock)
			{
				if (server == null)
					throw new InvalidOperationException("refcount is already 0");

				refCount--;
				if (refCount == 0)

				{
					server.Dispose();
					Config.Dispose();
					ClusterManager.Shutdown(ClusterName);

					server = null;
					ClientConfig = null;
				}
			}
		}

		public SharedServerFixture()
		{
			ClientConfig = GetConfig();
		}

		public IContainer ClientConfig { get; private set; }

		public void Dispose()
		{
			ReleaseConfig();
		}
	}
}
