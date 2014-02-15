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
			container.AutoWireAs<MemcachedNode, MemcachedNode, IPEndPoint>().ReusedWithin(ReuseScope.None);

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
				.Register<ICluster>(c => new MemcachedCluster(endpoints,
															c.Resolve<INodeLocator>(),
															c.Resolve<IReconnectPolicy>(),
															c.LazyResolve<MemcachedNode, System.Net.IPEndPoint>()))
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

#region [ License information          ]

/* ************************************************************
 *
 *    Copyright (c) Attila Kiskó, enyim.com
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/

#endregion
