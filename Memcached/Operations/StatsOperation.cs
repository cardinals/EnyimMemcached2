using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class StatsOperation : MemcachedOperationBase<INodeStatsOperationResult>, IStatsOperation
	{
		private static readonly Enyim.Caching.ILog log = Enyim.Caching.LogManager.GetLogger(typeof(StatsOperation));

		private string type;
		private Dictionary<string, string> stats;

		public StatsOperation(string type)
		{
			this.type = type;
		}

		protected override BinaryRequest CreateRequest()
		{
			var request = new BinaryRequest(OpCode.Stat);
			if (!String.IsNullOrEmpty(type))
				request.Key = BinaryConverter.EncodeKey(type);

			return request;
		}

		protected override INodeStatsOperationResult CreateResult(BinaryResponse response)
		{
			if (stats == null)
				stats = new Dictionary<string, string>();

			var data = response.Data;

			// if empty key (last response packet response)
			// or if error
			// return the response object to break the loop and let the node process the next op.
			if (response.KeyLength == 0 || !response.Success)
			{
				return new NodeStatsOperationResult { Value = stats }.WithResponse(response);
			}

			// decode stat key/value
			var key = BinaryConverter.DecodeKey(data.Array, data.Offset, response.KeyLength);
			var value = BinaryConverter.DecodeKey(data.Array, data.Offset + response.KeyLength, data.Count - response.KeyLength);

			stats[key] = value;

			return null; // we expect more response packets
		}
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
