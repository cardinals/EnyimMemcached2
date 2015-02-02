using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class ClusterBuilderTests
	{
		[Fact]
		public void Should_Not_Register_Duplicates()
		{
			const string Name = "Should_Not_Register_Duplicates";
			new ClusterBuilder(Name).Register();
			var e = Assert.Throws<ArgumentException>(() => new ClusterBuilder(Name).Register());

			Assert.Equal("Cluster is already registered: " + Name, e.Message);
			ClusterManager.Shutdown(Name);
		}

		[Fact]
		public void Cluster_With_Same_Name_Can_Be_Registered_When_Previous_Instance_Is_Stopped()
		{
			const string Name = "Cluster_With_Same_Name_Can_Be_Registered_When_Previous_Instance_Is_Stopped";

			for (var i = 0; i < 3; i++)
			{
				new ClusterBuilder(Name).Register();
				ClusterManager.Shutdown(Name);
			}
		}

		[Fact]
		public void Can_Build_Using_Extensions()
		{
			// Register a StepRecorder instance which will be used
			// by the fake services to mark themselves initialized.
			// If any of them is missing from the StepRecorder,
			// the ClusterBuilder is not initializing a service.
			const string Name = "Can_Build_Using_Extensions";
			var c = new ClusterBuilder(Name)
							.Endpoints("127.0.0.1")
							.Use
								.Service<StepRecorder>()
								.FailurePolicy<_FailurePolicy>()
								.NodeLocator<_NodeLocator>()
								.ReconnectPolicy<_ReconnectPolicy>()
							.Register();

			var cluster = c.Resolve<ICluster>();
			var m = c.Resolve<StepRecorder>();

			m.AssertSteps("FailurePolicy", "NodeLocator", "ReconnectPolicy");

			ClusterManager.Shutdown(Name);
		}

		[Fact]
		public void Can_Build_From_Config()
		{
			const string Name = "Can_Build_From_Config";
			// Register a StepRecorder instance which will be used
			// by the fake services to mark themselves initialized.
			// If any of them is missing from the StepRecorder,
			// the ClusterBuilder is not initializing a service.
			var c = new ClusterBuilder(Name)
							.FromConfiguration("Can_Build_From_Config")
							.Use
								.Service<StepRecorder>()
							.Register();

			var cluster = c.Resolve<ICluster>();
			var m = c.Resolve<StepRecorder>();

			m.AssertSteps("FailurePolicy", "NodeLocator", "ReconnectPolicy");

			Assert.IsType<_FailurePolicy>(c.Resolve<IFailurePolicy>());
			Assert.IsType<_NodeLocator>(c.Resolve<INodeLocator>());
			Assert.IsType<_ReconnectPolicy>(c.Resolve<IReconnectPolicy>());

			Assert.Equal(10, ((_FailurePolicy)c.Resolve<IFailurePolicy>()).Property);
			Assert.Equal("10", ((_NodeLocator)c.Resolve<INodeLocator>()).Property);
			Assert.Equal(TimeSpan.FromSeconds(10), ((_ReconnectPolicy)c.Resolve<IReconnectPolicy>()).Property);

			ClusterManager.Shutdown(Name);
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
