using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Configuration;

namespace Enyim.Caching.Memcached.Configuration
{
	public static class ClusterBuilderExtensions
	{
		public const int DefaultPort = 11211;

		public static IClusterBuilderServicesNext NodeLocator<T>(this IClusterBuilderServices services, Func<INodeLocator> factory)
		{
			return services.Service(factory);
		}

		public static IClusterBuilderServicesNext FailurePolicy(this IClusterBuilderServices services, Func<IFailurePolicy> factory)
		{
			return services.Service(factory);
		}

		public static IClusterBuilderServicesNext ReconnectPolicy(this IClusterBuilderServices services, Func<IReconnectPolicy> factory)
		{
			return services.Service(factory);
		}

		public static IClusterBuilderNext SocketOpts(this IClusterBuilderNext services, int? sendBufferSize = null, int? receiveBufferSize = null,
																TimeSpan? connectionTimeout = null, TimeSpan? sendTimeout = null, TimeSpan? receiveTimeout = null)
		{
			services.Use.Service<Func<ISocket>>(() => () =>
			{
				var retval = new AsyncSocket();

				if (sendBufferSize != null) retval.SendBufferSize = sendBufferSize.Value;
				if (receiveBufferSize != null) retval.ReceiveBufferSize = receiveBufferSize.Value;
				if (connectionTimeout != null) retval.ConnectionTimeout = connectionTimeout.Value;
				if (sendTimeout != null) retval.SendTimeout = sendTimeout.Value;
				if (receiveTimeout != null) retval.ReceiveTimeout = receiveTimeout.Value;

				return retval;
			});

			return services;
		}

		public static IClusterBuilderNext Endpoints(this IClusterBuilder builder, params string[] endpoints)
		{
			return builder.Endpoints(endpoints.Select(e => ConfigurationHelper.ParseEndPoint(e, DefaultPort)));
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
