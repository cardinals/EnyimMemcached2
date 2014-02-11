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
