using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	public sealed class ProviderElement<T> : ConfigurationElement
		where T : class
	{
		public ProviderElement()
		{
			Parameters = new Dictionary<string, string>();
		}

		public Dictionary<string, string> Parameters { get; private set; }

		/// <summary>
		/// Gets or sets the type of the provider.
		/// </summary>
		[ConfigurationProperty("type", IsRequired = true), TypeConverter(typeof(TypeNameConverter))]
		public Type Type
		{
			get { return (Type)base["type"]; }
			set
			{
				ConfigurationHelper.CheckForInterface(value, typeof(T));
				base["type"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			var property = new ConfigurationProperty(name, typeof(string), value);
			base[property] = value;
			Parameters[name] = value;

			return true;
		}

		[ConfigurationProperty("data", IsRequired = false)]
		public TextElement Content
		{
			get { return (TextElement)base["data"]; }
			set { base["data"] = value; }
		}

		protected override void PostDeserialize()
		{
			base.PostDeserialize();

			var c = this.Content;
			if (c != null)
				Parameters[String.Empty] = c.Content;
		}
	}
}
