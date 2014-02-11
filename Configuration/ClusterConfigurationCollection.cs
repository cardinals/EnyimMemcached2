using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace Enyim.Caching.Configuration
{
	public class ClusterConfigurationCollection : ConfigurationElementCollection
	{
		protected override void Init()
		{
			AddElementName = "cluster";

			base.Init();
		}

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ClusterConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ClusterConfigurationElement)element).Name;
		}

		public ClusterConfigurationElement ByName(string name)
		{
			var retval = BaseGet(name) as ClusterConfigurationElement;
			if (retval == null)
				throw new KeyNotFoundException("cluster '" + name + "' not found");

			return retval;
		}
	}
}
