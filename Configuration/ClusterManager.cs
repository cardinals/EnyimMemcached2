using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	public static class ClusterManager
	{
		private const string DefaultName = "<default>";
		private static readonly ConcurrentDictionary<string, Container> ClusterCache = new ConcurrentDictionary<string, Container>();

		public static bool IsRegistered(string name)
		{
			return ClusterCache.ContainsKey(name ?? String.Empty);
		}

		public static void Shutdown(string name)
		{
			Container c;

			if (!ClusterCache.TryRemove(name ?? String.Empty, out c))
				throw NotRegistered(name);

			c.Dispose();
		}

		internal static void CacheCluster(string name, Container container)
		{
			if (!ClusterCache.TryAdd(name ?? String.Empty, container))
				throw AlreadyExists(name);
		}

		internal static Container GetCluster(string name)
		{
			Container retval;

			if (!ClusterCache.TryGetValue(name ?? String.Empty, out retval))
				throw NotRegistered(name);

			return retval;
		}

		private static string FixDisplayName(string name)
		{
			return String.IsNullOrEmpty(name) ? DefaultName : name;
		}

		private static ArgumentException AlreadyExists(string name)
		{
			return new ArgumentException("Cluster is already registered: " + FixDisplayName(name)); ;
		}

		private static ArgumentException NotRegistered(string name)
		{
			return new ArgumentException("Cluster is not registered: " + FixDisplayName(name)); ;
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
