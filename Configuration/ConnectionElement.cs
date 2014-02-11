using System;
using System.ComponentModel;
using System.Configuration;

namespace Enyim.Caching.Configuration
{
	public class ConnectionElement : ConfigurationElement
	{
		[ConfigurationProperty("bufferSize", IsRequired = false, DefaultValue = 16384)]
		[IntegerValidator(MinValue = 1)]
		public int BufferSize
		{
			get { return (int)base["bufferSize"]; }
			set { base["bufferSize"] = value; }
		}

		[ConfigurationProperty("timeout", IsRequired = false, DefaultValue = "00:00:10")]
		[PositiveTimeSpanValidator]
		[TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan Timeout
		{
			get { return (TimeSpan)base["timeout"]; }
			set { base["timeout"] = value; }
		}
	}
}
