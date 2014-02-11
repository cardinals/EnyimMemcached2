using System;
using System.Collections.Generic;
using Enyim.Caching.Configuration;

namespace Enyim.Caching
{
	public class PeriodicReconnectPolicy : IReconnectPolicy, ISupportInitialize
	{
		public PeriodicReconnectPolicy()
		{
			Interval = TimeSpan.FromSeconds(10);
		}

		public TimeSpan Interval { get; set; }

		public TimeSpan Schedule(INode node)
		{
			return Interval;
		}

		public void Reset(INode node)
		{
		}

		void ISupportInitialize.Initialize(IDictionary<string, string> properties)
		{
			Interval = ConfigurationHelper.GetAndRemove(properties, "interval", false, Interval);

			ConfigurationHelper.CheckForUnknownAttributes(properties);
		}
	}
}
