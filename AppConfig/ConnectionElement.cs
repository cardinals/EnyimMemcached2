using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Enyim.Caching.Configuration
{
	public class ConnectionElement : ConfigurationElement
	{
		private static readonly ConfigurationProperty sendBufferSize;
		private static readonly ConfigurationProperty receiveBufferSize;

		private static readonly ConfigurationProperty connectionTimeout;
		private static readonly ConfigurationProperty sendTimeout;
		private static readonly ConfigurationProperty receiveTimeout;

		private static readonly ConfigurationPropertyCollection properties;

		static ConnectionElement()
		{
			sendBufferSize = new ConfigurationProperty("sendBufferSize", typeof(int), AsyncSocket.Defaults.SendBufferSize, null, new IntegerValidator(AsyncSocket.Defaults.MinBufferSize, AsyncSocket.Defaults.MaxBufferSize), ConfigurationPropertyOptions.None);
			receiveBufferSize = new ConfigurationProperty("receiveBufferSize", typeof(int), AsyncSocket.Defaults.ReceiveBufferSize, null, new IntegerValidator(AsyncSocket.Defaults.MinBufferSize, AsyncSocket.Defaults.MaxBufferSize), ConfigurationPropertyOptions.None);

			connectionTimeout = new ConfigurationProperty("connectionTimeout", typeof(TimeSpan), TimeSpan.FromMilliseconds(AsyncSocket.Defaults.ConnectionTimeoutMsec), new InfiniteTimeSpanConverter(), new PositiveTimeSpanValidator(), ConfigurationPropertyOptions.None);
			sendTimeout = new ConfigurationProperty("sendTimeout", typeof(TimeSpan), TimeSpan.FromMilliseconds(AsyncSocket.Defaults.ConnectionTimeoutMsec), new InfiniteTimeSpanConverter(), new PositiveTimeSpanValidator(), ConfigurationPropertyOptions.None);
			receiveTimeout = new ConfigurationProperty("receiveTimeout", typeof(TimeSpan), TimeSpan.FromMilliseconds(AsyncSocket.Defaults.ConnectionTimeoutMsec), new InfiniteTimeSpanConverter(), new PositiveTimeSpanValidator(), ConfigurationPropertyOptions.None);

			properties = new ConfigurationPropertyCollection { sendBufferSize, receiveBufferSize, connectionTimeout, sendTimeout, receiveTimeout };
		}

		protected override ConfigurationPropertyCollection Properties
		{
			get { return properties; }
		}

		public int SendBufferSize
		{
			get { return (int)base[sendBufferSize]; }
		}

		public int ReceiveBufferSize
		{
			get { return (int)base[receiveBufferSize]; }
		}

		public TimeSpan ConnectionTimeout
		{
			get { return (TimeSpan)base[connectionTimeout]; }
		}

		public TimeSpan SendTimeout
		{
			get { return (TimeSpan)base[sendTimeout]; }
		}

		public TimeSpan ReceiveTimeout
		{
			get { return (TimeSpan)base[receiveTimeout]; }
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
