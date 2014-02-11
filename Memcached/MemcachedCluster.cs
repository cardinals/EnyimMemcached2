using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class MemcachedCluster : BasicCluster
	{
		private readonly Func<IPEndPoint, INode> nodeFactory;

		public MemcachedCluster(IEnumerable<IPEndPoint> endpoints, INodeLocator locator, IReconnectPolicy policy, Func<IPEndPoint, INode> nodeFactory)
			: base(endpoints, locator, policy)
		{
			this.nodeFactory = nodeFactory;
		}

		protected override INode CreateNode(IPEndPoint endpoint)
		{
			return nodeFactory(endpoint);
		}
	}
}
