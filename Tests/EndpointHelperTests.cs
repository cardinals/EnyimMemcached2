using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Enyim.Caching.Configuration;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class EndpointHelperTests
	{
		[Fact]
		public void Parse_Localhost_Without_Port()
		{
			var ep = EndpointHelper.ParseEndPoint("localhost", 11211);
			Assert.Equal(11211, ep.Port);
			Assert.Equal(IPAddress.Loopback, ep.Address);
		}

		[Fact]
		public void Parse_IP_Without_Port()
		{
			var ep = EndpointHelper.ParseEndPoint("10.0.10.10", 11211);
			Assert.Equal(11211, ep.Port);
			Assert.Equal(new byte[] { 10, 0, 10, 10 }, ep.Address.GetAddressBytes());
		}

		[Fact]
		public void Parse_IP_With_Port()
		{
			var ep = EndpointHelper.ParseEndPoint("10.0.10.20:1234");
			Assert.Equal(1234, ep.Port);
			Assert.Equal(new byte[] { 10, 0, 10, 20 }, ep.Address.GetAddressBytes());
		}

		[Fact]
		public void Parse_Localhost_With_Port()
		{
			var ep = EndpointHelper.ParseEndPoint("localhost:1234");
			Assert.Equal(1234, ep.Port);
			Assert.Equal(IPAddress.Loopback, ep.Address);
		}

		[Fact]
		public void Invalid_Addresses_Should_Throw()
		{
			Assert.Throws<ArgumentNullException>(() => EndpointHelper.ParseEndPoint(null));
			Assert.Throws<ArgumentException>(() => EndpointHelper.ParseEndPoint("address:port"));
			Assert.Throws<SocketException>(() => EndpointHelper.ParseEndPoint("1234.56.78.9:1111"));
			Assert.Throws<ArgumentOutOfRangeException>(() => EndpointHelper.ParseEndPoint("10.0.10.10:987654"));
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
