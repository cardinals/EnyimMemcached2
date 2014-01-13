using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Funq;

namespace Enyim.Caching.Configuration
{
	public abstract class ClusterFactoryBase : IClusterFactory
	{
		private Funq.Container container;
		private List<IPEndPoint> nodes;

		public ClusterFactoryBase()
		{
			nodes = new List<IPEndPoint>();

			container = new Container { DefaultReuse = ReuseScope.None };
			container.AutoWireAs<INodeLocator, DefaultNodeLocator>();
			container.AutoWireAs<IFailurePolicy, ImmediateFailurePolicy>();
			container.AutoWireAs<IReconnectPolicy, PeriodicReconnectPolicy>();

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
				.AutoWireAs<ICluster, DefaultCluster, IEnumerable<IPEndPoint>>()
				.ReusedWithin(ReuseScope.Container);

			container.Register<INode>(c => { throw new InvalidOperationException("Must register an INode implementation"); });
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
