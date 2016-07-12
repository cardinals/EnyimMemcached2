using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class StoreOperation : BinarySingleItemOperation<IOperationResult>, IStoreOperation
	{
		protected const int ExtraLength = 8;

		public StoreOperation(IBufferAllocator allocator, Key key) : base(allocator, key) { }

		public StoreMode Mode { get; set; }
		public CacheItem Value { get; set; }
		public uint Expires { get; set; }
		public bool Silent { get; set; }

		protected OpCode GetOpCode(StoreMode mode)
		{
			if (Silent)
			{
				switch (mode)
				{
					case StoreMode.Add: return OpCode.AddQ;
					case StoreMode.Replace: return OpCode.ReplaceQ;
					case StoreMode.Set: return OpCode.SetQ;
				}
			}
			else
			{
				switch (mode)
				{
					case StoreMode.Add: return OpCode.Add;
					case StoreMode.Replace: return OpCode.Replace;
					case StoreMode.Set: return OpCode.Set;
				}
			}

			throw new ArgumentOutOfRangeException(nameof(mode), $"StoreMode {mode} is unsupported");
		}

		protected override BinaryRequest CreateRequest()
		{
			var request = new BinaryRequest(Allocator, GetOpCode(Mode), ExtraLength)
			{
				Key = Key,
				Cas = Cas,
				Data = Value.Data.AsArraySegment()
			};

			var extra = request.Extra;
			var extraArray = extra.Array;
			var extraOffset = extra.Offset;
			Debug.Assert(extraArray != null);

			// store the extra values
			NetworkOrderConverter.EncodeUInt32(Value.Flags, extraArray, extraOffset);
			NetworkOrderConverter.EncodeUInt32(Expires, extraArray, extraOffset + 4);

			return request;
		}

		protected override IOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new BinaryOperationResult();

			return response == null
					? retval.Success(this)
					: retval.WithResponse(response);
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
