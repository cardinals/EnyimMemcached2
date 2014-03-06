using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using Enyim.Caching.Configuration;
using Funq;
using CM = System.Configuration.ConfigurationManager;

namespace Enyim.Caching.Memcached.Configuration
{
	public static class ConfigurationManager
	{
		private const string DefaultName = "<default>";
		private static readonly ConcurrentDictionary<string, Container> ClientCache = new ConcurrentDictionary<string, Container>();
		private static readonly ConcurrentDictionary<string, Container> ClusterCache = new ConcurrentDictionary<string, Container>();

		internal static void CacheCluster(string name, Container container)
		{
			if (!ClusterCache.TryAdd(name ?? String.Empty, container))
				throw new ArgumentException("Cluster already exists: " + (name ?? DefaultName));
		}

		internal static Container GetCluster(string name)
		{
			Container retval;

			if (!ClusterCache.TryGetValue(name ?? String.Empty, out retval))
				throw new ArgumentException("Cluster is not registered: " + (name ?? DefaultName));

			return retval;
		}

		internal static void CacheClient(string name, Container container)
		{
			if (!ClientCache.TryAdd(name ?? String.Empty, container))
				throw new ArgumentException("Client already exists: " + (name ?? DefaultName));
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
