using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	/// Represents the statistics of a Memcached node.
	/// </summary>
	public sealed class ServerStats
	{
		public static readonly ServerStats Empty = new ServerStats();

		/// <summary>
		/// Defines a value which indicates that the statstics should be retrieved for all servers in the pool.
		/// </summary>
		public static readonly IPEndPoint All = new IPEndPoint(IPAddress.Any, 0);

		private static readonly Enyim.Caching.ILog log = Enyim.Caching.LogManager.GetCurrentClassLogger();

		// Summable StatItems start at OpSum
		private const int OpSum = 0x100;
		#region readonly string[] StatKeys
		// should map to the StatItem enum
		private static readonly string[] KeyNames = 
		{
			"uptime",
			"time",
			"version",
			"curr_items",
			"total_items",
			"curr_connections",
			"total_connections",
			"connection_structures",
			"cmd_get",
			"cmd_set",
			"get_hits",
			"get_misses",
			"bytes",
			"bytes_read",
			"bytes_written",
			"limit_maxbytes",
		};
		#endregion

		private readonly Dictionary<IPEndPoint, Dictionary<string, string>> results;

		internal ServerStats()
		{
			this.results = new Dictionary<IPEndPoint, Dictionary<string, string>>();
		}

		internal ServerStats(Dictionary<IPEndPoint, Dictionary<string, string>> results)
		{
			this.results = results;
		}

		internal ServerStats(IEnumerable<Tuple<INode, Dictionary<string, string>>> results)
		{
			this.results = results.ToDictionary(t => t.Item1.EndPoint, t => t.Item2);
		}

		internal void Append(IPEndPoint endpoint, Dictionary<string, string> stats)
		{
			results[endpoint] = stats;
		}

		/// <summary>
		/// Gets a stat value for the specified server.
		/// </summary>
		/// <param name="server">The adress of the server. If <see cref="IPAddress.Any"/> is specified it will return the sum of all server stat values.</param>
		/// <param name="item">The stat to be returned</param>
		/// <returns>The value of the specified stat item</returns>
		public long GetValue(IPEndPoint server, StatKeys item)
		{
			var retval = 0L;

			// need cluster statistics
			if (server.Address == IPAddress.Any)
			{
				// check if we can sum the value for all servers
				if (((int)item & OpSum) != OpSum)
					throw new ArgumentException("The " + item + " values cannot be summarized");

				// sum & return
				foreach (var endpoint in results.Keys)
				{
					retval += GetValue(endpoint, item);
				}
			}
			else
			{
				// error check
				// asked for a specific server
				var tmp = GetRaw(server, item);
				if (String.IsNullOrEmpty(tmp))
					throw new ArgumentException("Item was not found: " + item);

				// return the value
				if (!Int64.TryParse(tmp, out retval))
					throw new InvalidOperationException("Item '" + item + "' is not a number. Original value: " + tmp);
			}

			return retval;
		}

		/// <summary>
		/// Returns the server of memcached running on the specified server.
		/// </summary>
		/// <param name="server">The adress of the server</param>
		/// <returns>The version of memcached</returns>
		public Version GetVersion(IPEndPoint server)
		{
			string version = GetRaw(server, StatKeys.Version);
			if (String.IsNullOrEmpty(version))
				throw new InvalidOperationException("No version found for the server " + server);

			return new Version(version);
		}

		/// <summary>
		/// Returns the uptime of the specific server.
		/// </summary>
		/// <param name="server">The adress of the server</param>
		/// <returns>A value indicating how long the server is running</returns>
		public TimeSpan GetUptime(IPEndPoint server)
		{
			string uptime = GetRaw(server, StatKeys.Uptime);
			if (String.IsNullOrEmpty(uptime))
				throw new InvalidOperationException("No uptime found for the server " + server);

			long value;
			if (!Int64.TryParse(uptime, out value))
				throw new InvalidOperationException("Invalid uptime string was returned: " + uptime);

			return TimeSpan.FromSeconds(value);
		}

		public IDictionary<IPEndPoint, string> GetRaw(string key)
		{
			string tmp;

			return this.results
						.ToDictionary(kvp => kvp.Key,
										kvp => kvp.Value.TryGetValue(key, out tmp)
												? tmp
												: null);
		}

		/// <summary>
		/// Returns the stat value for a specific server. The value is not converted but returned as the server returned it.
		/// </summary>
		/// <param name="server">The adress of the server</param>
		/// <param name="item">The stat value to be returned</param>
		/// <returns>The value of the stat item</returns>
		public string GetRaw(IPEndPoint server, StatKeys item)
		{
			var index = (int)item;

			if (index < 0 || index >= KeyNames.Length)
				throw new ArgumentOutOfRangeException("item");

			return GetRaw(server, KeyNames[index]);
		}

		/// <summary>
		/// Returns the stat value for a specific server. The value is not converted but returned as the server returned it.
		/// </summary>
		/// <param name="server">The adress of the server</param>
		/// <param name="key">The name of the stat value</param>
		/// <returns>The value of the stat item</returns>
		public string GetRaw(IPEndPoint server, string key)
		{
			Dictionary<string, string> serverValues;
			string retval;

			if (this.results.TryGetValue(server, out serverValues))
			{
				if (serverValues.TryGetValue(key, out retval))
					return retval;

				if (log.IsDebugEnabled)
					log.Debug("The stat item {0} does not exist for {1}", key, server);
			}
			else
			{
				if (log.IsDebugEnabled)
					log.Debug("No stats are stored for {0}", server);
			}

			return null;
		}
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    Copyright (c) 2010 Attila Kiskó, enyim.com
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
