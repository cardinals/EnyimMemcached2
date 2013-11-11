using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached.Results;
using Funq;

namespace Enyim.Caching.Memcached
{
	public class MemcachedClient : IDisposable
	{
		private ICluster cluster;
		private bool owns;
		private IMemcachedClientConfiguration configuration;

		//public MemcachedClient() : this(String.Empty) { }
		//public MemcachedClient(string clusterName) : this(String.Empty, ClusterManager.Get(clusterName)) { }
		//public MemcachedClient(string configName, string clusterName) : this(String.Empty, ClusterManager.Get(clusterName)) { }
		//public MemcachedClient( IClusterFactory clusterFactory) : this(clusterFactory.Create(), true) { }

		public MemcachedClient(IMemcachedClientConfiguration configuration, ICluster cluster) : this(configuration, cluster, false) { }

		private MemcachedClient(IMemcachedClientConfiguration configuration, ICluster cluster, bool owns)
		{
			this.cluster = cluster;
			this.configuration = configuration;
			this.owns = owns;
		}

		~MemcachedClient()
		{
			Dispose();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			if (owns && cluster != null)
			{
				cluster.Dispose();
				cluster = null;
			}
		}

		public async Task<object> Get(string key)
		{
			var tmp = await GetWithResult(key);

			return tmp.Value;
		}

		public async Task<IGetOperationResult> GetWithResult(string key)
		{
			var op = configuration.OperationFactory.Get(key);

			await cluster.Execute(op);

			return op.Result;
		}
	}
}
