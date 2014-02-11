using System;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using Enyim.Caching.Configuration;
using Funq;
using System.Diagnostics;
using System.Net;

namespace Enyim.Caching.Memcached.Configuration
{
	internal static class ContainerHelper
	{
		public static void RegisterInto(this ClientConfigurationSection section, Funq.Container container)
		{
			container.AutoWireAs<INode, MemcachedNode, IPEndPoint>();

			section.OperationFactory.RegisterInto(container, typeof(MemcachedOperationFactory));
			section.Transcoder.RegisterInto(container, typeof(DefaultTranscoder));
			section.PerformanceMonitor.RegisterInto(container, typeof(NullPerformanceMonitor));
			section.KeyTransformer.RegisterInto(container, typeof(NullKeyTransformer));
		}

		public static void RegisterInto(this ClusterConfigurationElement section, Funq.Container container)
		{
			var bs = section.Connection.BufferSize;
			var to = section.Connection.Timeout;

			container
				.AutoWireAs<ISocket, SafeSocket>()
				.InitializedBy((c, socket) =>
				{
					socket.BufferSize = bs;
					socket.ConnectionTimeout = to;
				})
				.ReusedWithin(ReuseScope.None);

			// TODO figure out component lifetimes
			section.NodeLocator.RegisterInto(container, typeof(DefaultNodeLocator));
			section.FailurePolicy.RegisterInto(container, typeof(ImmediateFailurePolicy));
			section.ReconnectPolicy.RegisterInto(container, typeof(PeriodicReconnectPolicy));

			var endpoints = section.Nodes.AsIPEndPoints().ToArray();

			// such uglies
			container
				.Register<ICluster>(c => new DefaultCluster(endpoints,
															c.Resolve<INodeLocator>(),
															c.Resolve<IReconnectPolicy>(),
															c.LazyResolve<INode, System.Net.IPEndPoint>()))
				.InitializedBy((_, c) => c.Start())
				.ReusedWithin(ReuseScope.Container);
		}

		public static IRegistration<TContract> TryRegisterInto<TContract>(this ProviderElement<TContract> element, Container target)
			where TContract : class
		{
			return element.RegisterInto(target, null);
		}

		public static IRegistration<TContract> RegisterInto<TContract>(this ProviderElement<TContract> element, Container target, Type defaultImplementation)
			where TContract : class
		{
			var type = element == null || element.Type == null
						? defaultImplementation
						: element.Type;

			if (type == null) return null;

			var reg = target.AutoWireAs<TContract>(type);
			if (typeof(ISupportInitialize).IsAssignableFrom(type))
				reg.InitializedBy((c, instance) => ((ISupportInitialize)instance).Initialize(element.Parameters));

			return reg;
		}
	}
}
