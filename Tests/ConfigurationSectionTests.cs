using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class ConfigurationSectionTests
	{
		[Fact]
		public void Can_Load_Client_Section()
		{
			var section = System.Configuration.ConfigurationManager.GetSection("enyim.com/memcached/client") as ClientConfigurationSection;
			Assert.NotNull(section);

			Assert.Equal(typeof(_Operationfactory), section.OperationFactory.Type);
			Assert.Equal(typeof(_Transcoder), section.Transcoder.Type);
			Assert.Equal(typeof(_KeyTransformer), section.KeyTransformer.Type);
		}

		[Fact]
		public void Can_Load_Cluster_Section()
		{
			var section = System.Configuration.ConfigurationManager.GetSection("enyim.com/memcached/clusters") as ClustersConfigurationSection;
			Assert.NotNull(section);

			Assert.NotNull(section.Clusters.ByName(null));
			Assert.NotNull(section.Clusters.ByName(String.Empty));
			Assert.Equal(section.Clusters.ByName(null), section.Clusters.ByName(String.Empty));

			Assert.NotNull(section.Clusters.ByName("Can_Build_From_Config"));
			Assert.NotNull(section.Clusters.ByName("second"));

			Assert.Throws<KeyNotFoundException>(() => section.Clusters.ByName("missing"));
		}
	}

}
