﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Enyim.Caching.Memcached.Configuration
{
	public static class ClientConfigurationBuilderExtensions
	{
		public static IClientBuilderServicesNext Transcoder(this IClientBuilderServices services, Func<IContainer, ITranscoder> factory)
		{
			return services.Service(factory);
		}

		public static IClientBuilderServicesNext Transcoder<TService>(this IClientBuilderServices services, Action<ITranscoder> initializer = null)
		{
			return services.Service(typeof(TService), initializer);
		}

		public static IClientBuilderServicesNext KeyTransformer(this IClientBuilderServices services, Func<IContainer, IKeyTransformer> factory)
		{
			return services.Service(factory);
		}

		public static IClientBuilderServicesNext KeyTransformer<TService>(this IClientBuilderServices services, Action<IKeyTransformer> initializer = null)
		{
			return services.Service(typeof(TService), initializer);
		}

		public static IClientBuilderServicesNext OperationFactory(this IClientBuilderServices services, Func<IContainer, IOperationFactory> factory)
		{
			return services.Service(factory);
		}

		public static IClientBuilderServicesNext OperationFactory<TService>(this IClientBuilderServices services, Action<IOperationFactory> initializer = null)
		{
			return services.Service(typeof(TService), initializer);
		}

		public static void MakeDefault(this IContainer self, bool force = false)
		{
			if (MemcachedClientBase.DefaultContainer != null)
			{
				if (!force)
					throw new InvalidOperationException("There is already a default configuration defined for MemcachedClient");

				MemcachedClientBase.DefaultContainer.Dispose();
			}

			MemcachedClientBase.DefaultContainer = self;
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
