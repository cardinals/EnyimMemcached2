using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Funq;

namespace Enyim.Caching.Memcached
{
	public class BasicConfiguration : IClusterFactory
	{
		private Funq.Container container;
		private List<IPEndPoint> nodes;

		public BasicConfiguration()
		{
			nodes = new List<IPEndPoint>();

			container = new Container { DefaultReuse = ReuseScope.None };
			container.AutoWireAs<INodeLocator, DefaultNodeLocator>();
			container.AutoWireAs<INodeFailurePolicy, ImmediateFailurePolicy>();
			container.AutoWireAs<IReconnectPolicy, SimpleReconnectPolicy>();
			container.AutoWireAs<IKeyTransformer, NullKeyTransformer>();

			// cluster + nodes
			container
				.Register<INode, IPEndPoint>((c, ip) => new BinaryNode(ip, c.Resolve<INodeFailurePolicy>()))
				.InitializedBy((c, n) =>
				{
					BufferSize = BufferSize;
				});

			container.AutoWireAs<ICluster, MemcachedCluster, IEnumerable<IPEndPoint>>();

			// socket
			container
				.Register<SafeSocket, IPEndPoint>((c, p) => new SafeSocket(p))
				.InitializedBy((c, s) =>
				{
					s.ConnectionTimeout = ConnectionTimeout;
					s.ReceiveTimeout = ConnectionTimeout;
				});
		}

		public IList<IPEndPoint> Nodes { get { return nodes; } }
		public TimeSpan ConnectionTimeout { get; set; }
		public int BufferSize { get; set; }
		public Funq.Container Container { get { return container; } }

		internal void AddNodes(IEnumerable<IPEndPoint> nodes)
		{
			this.nodes.AddRange(nodes);
		}

		public ICluster Create()
		{
			return container.Resolve<ICluster, IEnumerable<IPEndPoint>>(nodes);
		}
	}
}
