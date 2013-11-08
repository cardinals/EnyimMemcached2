using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
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
			container.AutoWireAs<IFailurePolicy, ImmediateFailurePolicy>();
			container.AutoWireAs<IReconnectPolicy, PeriodicReconnectPolicy>();
			container.AutoWireAs<IKeyTransformer, NullKeyTransformer>();

			// socket
			container
				.AutoWireAs<ISocket, SafeSocket>()
				.InitializedBy((c, s) =>
				{
					s.ConnectionTimeout = ConnectionTimeout;
					s.ReceiveTimeout = ConnectionTimeout;
				});

			// cluster + nodes
			container
				.AutoWireAs<INode, MemcachedNode, IPEndPoint>()
				.InitializedBy((c, n) =>
				{
					BufferSize = BufferSize;
				});

			container
				.AutoWireAs<ICluster, DefaultCluster, IEnumerable<IPEndPoint>>()
				.ReusedWithin(ReuseScope.Container);
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
