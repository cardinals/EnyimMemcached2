using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	internal static class ContainerHelper
	{
		/// <summary>
		/// Adds the default cluster services to a container.
		/// </summary>
		/// <param name="container"></param>
		internal static void AddClusterDefauls(this Container container)
		{
			container.Register<ICluster>(_ => { throw new InvalidOperationException("Must register an ICluster implementation!"); });
			container.Register<Func<ISocket>>(_ => () => new AsyncSocket());

			container.AutoWireAs<INodeLocator, DefaultNodeLocator>();
			container.AutoWireAs<IFailurePolicy, ImmediateFailurePolicy>();
			container.AutoWireAs<IReconnectPolicy, PeriodicReconnectPolicy>();

#if TRACK_ALLOCATIONS
			container.Register<IBufferAllocator>(new TrackingBufferAllocator(1024 * 1024, 1024 * 1024 * 20));
#else
			container.Register<IBufferAllocator>(new BufferManagerAllocator());
#endif
		}

		/// <summary>
		/// Adds the default client services to a container.
		/// </summary>
		/// <param name="container"></param>
		internal static void AddClientDefauls(this Container container)
		{
			container.AutoWireAs<IOperationFactory, DefaultOperationFactory>();
			container.AutoWireAs<ITranscoder, DefaultTranscoder>();
			container.AutoWireAs<IPerformanceMonitor, NullPerformanceMonitor>();
			container.AutoWireAs<IKeyTransformer, NullKeyTransformer>();
		}

		internal static void RegisterCluster(this Container container, IEnumerable<IPEndPoint> endpoints)
		{
			var endpointsSnapshot = endpoints.ToArray();

			// such uglies
			container
				.Register<ICluster>(c =>
					new MemcachedCluster(endpointsSnapshot,
						c.Resolve<IBufferAllocator>(),
						c.Resolve<INodeLocator>(), c.Resolve<IReconnectPolicy>(),
						c.Resolve<IFailurePolicy>(), c.Resolve<Func<ISocket>>()))
				.InitializedBy((_, cluster) => cluster.Start());
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
