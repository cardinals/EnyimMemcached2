using System;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	public class ClusterConfigurationSection : ConfigurationSection
	{
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

		[ConfigurationProperty("keyTransformer")]
		public ProviderElement<IKeyTransformer> KeyTransformer
		{
			get { return (ProviderElement<IKeyTransformer>)base["keyTransformer"]; }
			set { base["keyTransformer"] = value; }
		}

		[ConfigurationProperty("failurePolicy")]
		public ProviderElement<INodeFailurePolicy> FailurePolicy
		{
			get { return (ProviderElement<INodeFailurePolicy>)base["failurePolicy"]; }
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
