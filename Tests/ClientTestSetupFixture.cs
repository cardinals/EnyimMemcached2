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
		private Process process;

		public ClientTestSetupFixture()
		{
			var path = AppDomain.CurrentDomain.BaseDirectory;
			path = Path.Combine(path, "Tools");

			process = Process.Start(new ProcessStartInfo
			{
				Arguments = "-E default_engine.so",
				FileName = path + "\\memcached.exe",
				WorkingDirectory = path,
				WindowStyle = ProcessWindowStyle.Hidden
			});

			new ClusterBuilder(ClusterName).FromConfiguration().Register();
			ClientConfig = new ClientConfigurationBuilder().Cluster(ClusterName).Create();
		}

		public IContainer ClientConfig { get; private set; }

		public void Dispose()
		{
			ClientConfig.Dispose();
			ClientConfig = null;
			ClusterManager.Shutdown(ClusterName);
			process.Kill();
		}
	}
}
