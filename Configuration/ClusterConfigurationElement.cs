using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace Enyim.Caching.Configuration
{
	public class ClusterConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, DefaultValue = "")]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}

		[ConfigurationProperty("locator")]
		public ProviderElement<INodeLocator> NodeLocator
		{
			get { return (ProviderElement<INodeLocator>)base["locator"]; }
			set { base["locator"] = value; }
		}

		[ConfigurationProperty("reconnectPolicy")]
		public ProviderElement<IReconnectPolicy> ReconnectPolicy
		{
			get { return (ProviderElement<IReconnectPolicy>)base["reconnectPolicy"]; }
			set { base["reconnectPolicy"] = value; }
		}

		[ConfigurationProperty("failurePolicy")]
		public ProviderElement<IFailurePolicy> FailurePolicy
		{
			get { return (ProviderElement<IFailurePolicy>)base["failurePolicy"]; }
			set { base["failurePolicy"] = value; }
		}

		[ConfigurationProperty("nodes")]
		public NodeElementCollection Nodes
		{
			get { return (NodeElementCollection)base["nodes"]; }
			set { base["nodes"] = value; }
		}

		[ConfigurationProperty("connection")]
		public ConnectionElement Connection
		{
			get { return (ConnectionElement)base["connection"]; }
			set { base["connection"] = value; }
		}
	}
}
