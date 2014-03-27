using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;

namespace Enyim.Caching.Configuration
{
	public class ClusterConfigurationCollection : ConfigurationElementCollection
	{
		private const string DefaultName = "<default>";

		public ClusterConfigurationElement ByName(string name)
		{
			var retval = BaseGet(name ?? String.Empty) as ClusterConfigurationElement;
			if (retval == null)
				throw new KeyNotFoundException("cluster '" + (String.IsNullOrEmpty(name) ? DefaultName : name) + "' not found");

			return retval;
		}

		#region [ Overrides                    ]

		public override ConfigurationElementCollectionType CollectionType
		{
			get { return ConfigurationElementCollectionType.AddRemoveClearMap; }
		}

		protected override void Init()
		{
			AddElementName = "cluster";

			base.Init();
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new ClusterConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((ClusterConfigurationElement)element).Name;
		}

		#endregion
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
