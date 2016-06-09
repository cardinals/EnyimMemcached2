using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class MutateOperation : BinarySingleItemOperation<IMutateOperationResult>, IMutateOperation
	{
		protected const int ExtraLength = 20;
		protected const int ResultLength = 8;

		private static readonly OpCode[] SilentOps = { OpCode.IncrementQ, OpCode.DecrementQ };
		private static readonly OpCode[] LoudOps = { OpCode.Increment, OpCode.Decrement };

		private OpCode[] operations = LoudOps;
		private bool silent;

		public MutateOperation(IBufferAllocator allocator, MutationMode mode, Key key)
			: base(allocator, key)
		{
			Mode = mode;
		}

		public MutationMode Mode { get; private set; }
		public ulong DefaultValue { get; set; }
		public ulong Delta { get; set; }
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
			var request = new BinaryRequest(Allocator, operations[(int)Mode], ExtraLength) { Key = Key, Cas = Cas };

			// store the extra values
			var extra = request.Extra.Array;
			var offset = request.Extra.Offset;
			NetworkOrderConverter.EncodeUInt64(Delta, extra, offset);
			NetworkOrderConverter.EncodeUInt64(DefaultValue, extra, offset + 8);
			NetworkOrderConverter.EncodeUInt32(Expires, extra, offset + 16);

			return request;
		}

		protected override IMutateOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new MutateOperationResult();

			if (response == null)
			{
				return Silent
						? retval.Success(this)
						: retval.NotFound(this);
			}

			if (response.Success)
			{
				var data = response.Data;
				if (data.Count != ResultLength)
					return retval.Failed(this, new InvalidOperationException("Result must be " + ResultLength + " bytes long, received: " + data.Count));

				retval.Value = NetworkOrderConverter.DecodeUInt64(data.Array, data.Offset);
			}

			return retval.WithResponse(response);
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
