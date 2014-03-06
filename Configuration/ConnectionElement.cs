using System;
using System.ComponentModel;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	public class ConnectionElement : ConfigurationElement
	{
		[IntegerValidator(MinValue = AsyncSocket.Defaults.MinBufferSize, MaxValue = AsyncSocket.Defaults.MaxBufferSize)]
		[ConfigurationProperty("sendBufferSize", IsRequired = false, DefaultValue = AsyncSocket.Defaults.SendBufferSize)]
		public int SendBufferSize
		{
			get { return (int)base["sendBufferSize"]; }
			set { base["sendBufferSize"] = value; }
		}

		[IntegerValidator(MinValue = AsyncSocket.Defaults.MinBufferSize, MaxValue = AsyncSocket.Defaults.MaxBufferSize)]
		[ConfigurationProperty("receiveBufferSize", IsRequired = false, DefaultValue = AsyncSocket.Defaults.ReceiveBufferSize)]
		public int ReceiveBufferSize
		{
			get { return (int)base["receiveBufferSize"]; }
			set { base["receiveBufferSize"] = value; }
		}

		[ConfigurationProperty("connectionTimeout", IsRequired = false, DefaultValue = AsyncSocket.Defaults.ConnectionTimeoutMsec)]
		[PositiveTimeSpanValidator]
		[TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan ConnectionTimeout
		{
			get { return (TimeSpan)base["connectionTimeout"]; }
			set { base["connectionTimeout"] = value; }
		}

		[PositiveTimeSpanValidator]
		[TypeConverter(typeof(InfiniteTimeSpanConverter))]
		[ConfigurationProperty("sendTimeout", IsRequired = false, DefaultValue = AsyncSocket.Defaults.SendTimeoutMsec)]
		public TimeSpan SendTimeout
		{
			get { return (TimeSpan)base["sendTimeout"]; }
			set { base["sendTimeout"] = value; }
		}

		[PositiveTimeSpanValidator]
		[TypeConverter(typeof(InfiniteTimeSpanConverter))]
		[ConfigurationProperty("receiveTimeout", IsRequired = false, DefaultValue = AsyncSocket.Defaults.ReceiveTimeoutMsec)]
		public TimeSpan ReceiveTimeout
		{
			get { return (TimeSpan)base["receiveTimeout"]; }
			set { base["receiveTimeout"] = value; }
		}

		internal ConnectionParams Clone()
		{
			return new ConnectionParams
			{
				SendBufferSize = SendBufferSize,
				ReceiveBufferSize = ReceiveBufferSize,
				ConnectionTimeout = ConnectionTimeout,
				SendTimeout = SendTimeout,
				ReceiveTimeout = ReceiveTimeout
			};
		}
	}

	internal class ConnectionParams
	{
		public int SendBufferSize { get; set; }
		public int ReceiveBufferSize { get; set; }
		public TimeSpan ConnectionTimeout { get; set; }
		public TimeSpan SendTimeout { get; set; }
		public TimeSpan ReceiveTimeout { get; set; }

		public void Update(ISocket socket)
		{
			socket.SendBufferSize = SendBufferSize;
			socket.ReceiveBufferSize = ReceiveBufferSize;
			socket.ConnectionTimeout = ConnectionTimeout;
			socket.SendTimeout = SendTimeout;
			socket.ReceiveTimeout = ReceiveTimeout;
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
