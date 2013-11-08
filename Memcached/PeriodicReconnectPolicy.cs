using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached
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

		void ISupportInitialize.Initialize(Dictionary<string, string> properties)
		{
			Interval = ConfigurationHelper.GetAndRemove(properties, "interval", false, Interval);

			ConfigurationHelper.CheckForUnknownAttributes(properties);
		}
	}
}
