using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Funq;

namespace Enyim.Caching.Integrations.Funq
{
	partial class ContainerWrapper : IContainer
	{
		private Container container;

		public ContainerWrapper(Container container)
		{
			this.container = container;
		}

		public TService Resolve<TService>()
		{
			return container.Resolve<TService>();
		}

		public TService ResolveNamed<TService>(string name)
		{
			return container.ResolveNamed<TService>(name);
		}
	}

	static partial class CHX
	{
		public static void AddDefaultServices(this Container self)
		{
			// cluster defaults
			self.AutoWireAs<ISocket, SafeSocket>();
			self.AutoWireAs<INodeLocator, DefaultNodeLocator>();
			self.AutoWireAs<IFailurePolicy, ImmediateFailurePolicy>();
			self.AutoWireAs<IReconnectPolicy, PeriodicReconnectPolicy>();

			// client defaults
			self.AutoWireAs<INode, MemcachedNode, IPEndPoint>();
			self.AutoWireAs<IKeyTransformer, NullKeyTransformer>();
			self.AutoWireAs<ITranscoder, DefaultTranscoder>();
			self.AutoWireAs<IOperationFactory, MemcachedOperationFactory>();
			self.AutoWireAs<IPerformanceMonitor, NullPerformanceMonitor>();

			// safeguard
			self.Register<ICluster>(_ => { throw new InvalidOperationException("Must register a cluster first"); });
		}

		public static void RegisterClusterFromConfig(this Container container, string sectionName)
		{
			var section = ConfigurationManager.GetSection(sectionName) as ClusterConfigurationSection;
			if (section == null)
				throw new ConfigurationErrorsException(String.Format("Section {0} was not found or it's not a ClusterConfigurationSection", sectionName));

			container
				.AutoWireAs<ISocket, SafeSocket>()
				.InitializedBy((c, socket) =>
				{
					socket.BufferSize = section.Connection.BufferSize;
					socket.ConnectionTimeout = section.Connection.Timeout;
				})
				.ReusedWithin(global::Funq.ReuseScope.None);

			section.FailurePolicy.TryRegisterInto(container);
			section.ReconnectPolicy.TryRegisterInto(container);
			section.NodeLocator.TryRegisterInto(container);

			CreateClusterRegistration(container, section.Nodes.ToIPEndPoints());
		}

		public static void RegisterClientSettingsFromConfig(this Container container, string sectionName)
		{
			var section = ConfigurationManager.GetSection(sectionName) as ClientConfigurationSection;
			if (section == null)
				throw new ConfigurationErrorsException(String.Format("Section {0} was not found or it's not a ClusterConfigurationSection", sectionName));

			section.OperationFactory.TryRegisterInto(container);
			section.Transcoder.TryRegisterInto(container);
			section.PerformanceMonitor.TryRegisterInto(container);
			section.KeyTransformer.TryRegisterInto(container);
		}

		private static IRegistration<ICluster> CreateClusterRegistration(Container container, IEnumerable<IPEndPoint> endpoints)
		{
			// such ugly
			Func<Container, ICluster> create =
				c => new DefaultCluster(endpoints, c.Resolve<INodeLocator>(), c.Resolve<IReconnectPolicy>(),
											c.LazyResolve<INode, System.Net.IPEndPoint>());

			var retval = container.Register<ICluster>(create);
			retval.InitializedBy((_, c) => c.Start()).ReusedWithin(ReuseScope.Container);

			return retval;
		}
	}

	/*
	public class AppSettingsClusterFactory : IClusterFactory
	{
		private string sectionName;
		private DefaultClusterFactory innerConfig;

		public AppSettingsClusterFactory() : this("enyim.com/memcached/default") { }

		public AppSettingsClusterFactory(string sectionName)
		{
			this.sectionName = sectionName;
		}

		public ICluster Create()
		{
			if (innerConfig == null)
			{
				var section = ConfigurationManager.GetSection(sectionName) as ClusterConfigurationSection;
				if (section == null)
					throw new ConfigurationErrorsException(String.Format("Section {0} was not found or it's not a ClusterConfigurationSection", sectionName));

				var config = new DefaultClusterFactory
				{
					BufferSize = section.Connection.BufferSize,
					ConnectionTimeout = section.Connection.Timeout
				};

				section.FailurePolicy.RegisterInto(config.Container);
				section.ReconnectPolicy.RegisterInto(config.Container);
				section.NodeLocator.RegisterInto(config.Container);

				config.AddNodes(section.Nodes.ToIPEndPoints());

				innerConfig = config;
			}

			return innerConfig.Create();
		}
	}
*/
}
