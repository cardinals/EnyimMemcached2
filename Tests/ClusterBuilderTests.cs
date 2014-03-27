using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class ClusterBuilderTests
	{
		[Fact]
		public void Should_Not_Register_Duplicates()
		{
			new ClusterBuilder("test").Register();
			var e = Assert.Throws<ArgumentException>(() => new ClusterBuilder("test").Register());

			Assert.Equal("Cluster is already registered: test", e.Message);
		}

		[Fact]
		public void Cluster_With_Same_Name_Can_Be_Registered_When_Previous_Instance_Is_Stopped()
		{
			new ClusterBuilder("name").Register();
			ClusterManager.Shutdown("name");
			new ClusterBuilder("name").Register();
		}

		[Fact]
		public void Can_Build_Using_Extensions()
		{
			var c = new ClusterBuilder("Can_Build_Using_Extensions")
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
		}

		[Fact]
		public void Can_Build_From_Config()
		{
			var c = new ClusterBuilder("Can_Build_From_Config")
							.FromConfiguration("Can_Build_From_Config")
							.Use.Service<StepRecorder>()
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
		}
	}


	

}
