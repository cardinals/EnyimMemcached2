using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class StoreOperation : BinarySingleItemOperation<IOperationResult>, IStoreOperation
	{
		protected const int ExtraLength = 8;

		private static readonly OpCode[] SilentOps = { OpCode.AddQ, OpCode.ReplaceQ, OpCode.SetQ };
		private static readonly OpCode[] LoudOps = { OpCode.Add, OpCode.Replace, OpCode.Set };

		private OpCode[] operations = LoudOps;
		private bool silent;

		public StoreOperation(IBufferAllocator allocator, StoreMode mode, Key key, CacheItem value)
			: base(allocator, key)
		{
			Mode = mode;
			Value = value;
		}

		public StoreMode Mode { get; private set; }
		public CacheItem Value { get; private set; }
		public uint Expires { get; set; }

		public bool Silent
		{
			get { return silent; }
			set
			{
				silent = value;
				operations = value ? SilentOps : LoudOps;
			}
		}

		protected override BinaryRequest CreateRequest()
		{
			var request = new StoreRequest(Allocator, operations[(int)Mode], Value, Expires)
			{
				Key = Key,
				Cas = Cas
			};

			return request;
		}

		protected override IOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new BinaryOperationResult();

			if (response == null)
				return retval.Success(this);

			return retval.WithResponse(response);
		}

		#region [ StoreRequest                 ]

		private class StoreRequest : BinaryRequest
		{
			public StoreRequest(IBufferAllocator allocator, OpCode operation, CacheItem value, uint expires)
				: base(allocator, operation, ExtraLength)
			{
				var extra = Extra.Array;
				var offset = Extra.Offset;

				// store the extra values
				NetworkOrderConverter.EncodeUInt32(value.Flags, extra, offset);
				NetworkOrderConverter.EncodeUInt32(expires, extra, offset + 4);

				Data = value.Segment.AsArraySegment();
			}
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
