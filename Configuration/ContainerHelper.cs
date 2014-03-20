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
		internal static void AddClusterDefauls(this Funq.Container container)
		{
			container.Register<Func<ISocket>>(_ => () => new AsyncSocket());
			container.AutoWireAs<INodeLocator, DefaultNodeLocator>();
			container.AutoWireAs<IFailurePolicy, ImmediateFailurePolicy>();
			container.AutoWireAs<IReconnectPolicy, PeriodicReconnectPolicy>();

			container.Register<ICluster>(_ => { throw new InvalidOperationException("Must register an ICluster implementation!"); });
		}

		internal static void AddClientDefauls(this Funq.Container container)
		{
			container.AutoWireAs<IOperationFactory, MemcachedOperationFactory>();
			container.AutoWireAs<ITranscoder, DefaultTranscoder>();
			container.AutoWireAs<IPerformanceMonitor, NullPerformanceMonitor>();
			container.AutoWireAs<IKeyTransformer, NullKeyTransformer>();
		}

		public static void RegisterInto(this ClientConfigurationSection section, Funq.Container container)
		{
			container.AddClientDefauls();

			container.RegisterFrom(section.OperationFactory);
			container.RegisterFrom(section.Transcoder);
			container.RegisterFrom(section.PerformanceMonitor);
			container.RegisterFrom(section.KeyTransformer);
		}

		public static void RegisterInto(this ClusterConfigurationElement section, Funq.Container container)
		{
			container.AddClusterDefauls();
			container.RegisterFrom(section.NodeLocator);
			container.RegisterFrom(section.FailurePolicy);
			container.RegisterFrom(section.ReconnectPolicy);
			container.RegisterCluster(section.Nodes.AsIPEndPoints());

			section.Connection.RegisterInto(container);
		}

		public static void RegisterInto(this ConnectionElement connectionElement, Funq.Container container)
		{
			if (connectionElement == null) return;
			var p = connectionElement.Clone();

			container.Register<Func<ISocket>>(_ => () =>
			{
				var retval = new AsyncSocket();
				p.Update(retval);

				return retval;
			});
		}

		internal static void RegisterCluster(this Container container, IEnumerable<IPEndPoint> endpoints)
		{
			var clone = endpoints.ToArray();

			// such uglies
			container
				.Register<ICluster>(c =>
					new MemcachedCluster(clone,
											c.Resolve<INodeLocator>(), c.Resolve<IReconnectPolicy>(),
											c.Resolve<IFailurePolicy>(), c.Resolve<Func<ISocket>>()))
				.InitializedBy((_, cluster) => cluster.Start());
		}

		public static IRegistration<TContract> RegisterFrom<TContract>(this Container container, ProviderElement<TContract> element, Type defaultImplementation = null)
			where TContract : class
		{
			var type = element == null || element.Type == null
						? defaultImplementation
						: element.Type;

			if (type == null) return null;

			var reg = container.AutoWireAs<TContract>(type);
			if (typeof(ISupportInitialize).IsAssignableFrom(type))
			{
				// make a snapshot of the parameters
				var clone = element.Parameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
				reg.InitializedBy((c, instance) => ((ISupportInitialize)instance).Initialize(clone));
			}

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
