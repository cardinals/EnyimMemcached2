using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public abstract class RemoteServerFixture
	{
		private const string ClusterName = "RemoteServerFixture";

		private static int InstanceCount;

		private IContainer config;

		public IContainer Config
		{
			get
			{
				InitConfig();
				return config;
			}
		}

		protected abstract void ConfigureServices(IClientBuilderServices services);

		protected void InitConfig()
		{
			var id = Interlocked.Increment(ref InstanceCount);
			var name = ClusterName + "-" + id;

			new ClusterBuilder(name)
					.Endpoints("10.2.4.10:11211")
					.Register();

			var configBuilder = new ClientConfigurationBuilder();
			ConfigureServices(configBuilder.Cluster(name).Use);

			config = configBuilder.Create();
		}

		public void Dispose()
		{
			if (config != null)
				config.Dispose();
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
