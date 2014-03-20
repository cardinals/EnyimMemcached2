using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Xunit;

namespace Tests
{
	public class ConfigurationHelperTests
	{
		[Fact]
		public void Parse_Localhost_Without_Port()
		{
			var ep = ConfigurationHelper.ParseEndPoint("localhost", 11211);
			Assert.Equal(11211, ep.Port);
			Assert.Equal(IPAddress.Loopback, ep.Address);
		}

		[Fact]
		public void Parse_IP_Without_Port()
		{
			var ep = ConfigurationHelper.ParseEndPoint("10.0.10.10", 11211);
			Assert.Equal(11211, ep.Port);
			Assert.Equal(new byte[] { 10, 0, 10, 10 }, ep.Address.GetAddressBytes());
		}

		[Fact]
		public void Parse_IP_With_Port()
		{
			var ep = ConfigurationHelper.ParseEndPoint("10.0.10.20:1234");
			Assert.Equal(1234, ep.Port);
			Assert.Equal(new byte[] { 10, 0, 10, 20 }, ep.Address.GetAddressBytes());
		}

		[Fact]
		public void Parse_Localhost_With_Port()
		{
			var ep = ConfigurationHelper.ParseEndPoint("localhost:1234");
			Assert.Equal(1234, ep.Port);
			Assert.Equal(IPAddress.Loopback, ep.Address);
		}

		[Fact]
		public void Invalid_Addresses_Should_Throw()
		{
			Assert.Throws<ArgumentNullException>(() => ConfigurationHelper.ParseEndPoint(null));
			Assert.Throws<ArgumentException>(() => ConfigurationHelper.ParseEndPoint("address:port"));
			Assert.Throws<SocketException>(() => ConfigurationHelper.ParseEndPoint("1234.56.78.9:1111"));
			Assert.Throws<ArgumentOutOfRangeException>(() => ConfigurationHelper.ParseEndPoint("10.0.10.10:987654"));
		}
	}
}
