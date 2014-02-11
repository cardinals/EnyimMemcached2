//using System;
//using System.Linq;
//using System.Configuration;
//using Funq;
//using System.Collections.Generic;

//namespace Enyim.Caching.Configuration
//{
//	public class ClusterConfigurationSection : ConfigurationSection
//	{
//		[ConfigurationProperty("locator")]
//		public ProviderElement<INodeLocator> NodeLocator
//		{
//			get { return (ProviderElement<INodeLocator>)base["locator"]; }
//			set { base["locator"] = value; }
//		}

//		[ConfigurationProperty("reconnectPolicy")]
//		public ProviderElement<IReconnectPolicy> ReconnectPolicy
//		{
//			get { return (ProviderElement<IReconnectPolicy>)base["reconnectPolicy"]; }
//			set { base["reconnectPolicy"] = value; }
//		}

//		[ConfigurationProperty("failurePolicy")]
//		public ProviderElement<IFailurePolicy> FailurePolicy
//		{
//			get { return (ProviderElement<IFailurePolicy>)base["failurePolicy"]; }
//			set { base["failurePolicy"] = value; }
//		}

//		[ConfigurationProperty("nodes")]
//		public NodeElementCollection Nodes
//		{
//			get { return (NodeElementCollection)base["nodes"]; }
//			set { base["nodes"] = value; }
//		}

//		[ConfigurationProperty("connection")]
//		public ConnectionElement Connection
//		{
//			get { return (ConnectionElement)base["connection"]; }
//			set { base["connection"] = value; }
//		}
//	}
//}

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
