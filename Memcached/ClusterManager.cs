using System;
using Funq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;

namespace Enyim.Caching
{
	public static class ClusterManager
	{
		private static readonly object NullKey = new Object();
		private static readonly ConcurrentDictionary<object, ICluster> clusters = new ConcurrentDictionary<object, ICluster>();

		public static ICluster Get()
		{
			return Get(null);
		}

		public static ICluster Get(string name)
		{
			ICluster retval;

			if (!clusters.TryGetValue(name ?? NullKey, out retval))
				throw new InvalidOperationException("cluster is not registered: " + (name ?? "<default>"));

			return retval;
		}

		public static ICluster Register(IClusterFactory factory)
		{
			return Register(null, factory);
		}

		public static ICluster Register(string name, IClusterFactory factory)
		{
			var retval = clusters.AddOrUpdate(name ?? NullKey,
												_ =>
												{
													var c = factory.Create();
													c.Start();

													return c;
												},
												(a, b) =>
												{
													throw new InvalidOperationException("cluster already exists: " + (name ?? "<default>"));
												});

			return retval;
		}

		public static void Remove(string name)
		{
			ICluster value;

			if (!clusters.TryRemove(name ?? NullKey, out value))
				throw new InvalidOperationException("cluster nbot found: " + (name ?? "<default>"));

			value.Dispose();
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
