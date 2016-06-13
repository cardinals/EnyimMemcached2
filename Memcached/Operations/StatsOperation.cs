using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class StatsOperation : MemcachedOperationBase<INodeStatsOperationResult>, IStatsOperation
	{
		private readonly string type;
		private Dictionary<string, string> stats;

		public StatsOperation(IBufferAllocator allocator, string type)
			: base(allocator)
		{
			this.type = type;
		}

		protected override BinaryRequest CreateRequest()
		{
			var request = new BinaryRequest(Allocator, OpCode.Stat);
			if (!String.IsNullOrEmpty(type))
				request.Key = EncodeKey(type);

			return request;
		}

		private Key EncodeKey(string key)
		{
			var array = Allocator.Take(key.Length);
			Encoding.ASCII.GetBytes(key, 0, key.Length, array, 0);

			return new Key(Allocator, array, key.Length);
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
			// both are ASCII in memcached
			var key = Encoding.ASCII.GetString(data.Array, data.Offset, response.KeyLength);
			var value = Encoding.ASCII.GetString(data.Array, data.Offset + response.KeyLength, data.Count - response.KeyLength);

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
