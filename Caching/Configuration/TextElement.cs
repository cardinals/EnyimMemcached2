using System;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	public class TextElement : ConfigurationElement
	{
		public string Content { get; set; }

		protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
		{
			this.Content = reader.ReadElementContentAsString();
		}
	}
}
