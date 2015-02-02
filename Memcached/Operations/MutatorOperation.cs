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

		private readonly ulong defaultValue;
		private readonly ulong delta;
		private readonly uint expires;

		public MutateOperation(MutationMode mode, byte[] key, ulong defaultValue, ulong delta, uint expires)
			: base(key)
		{
			Require.Value("delta", delta >= 0, "delta must be a positive integer");

			this.defaultValue = defaultValue;
			this.delta = delta;
			this.expires = expires;

			Mode = mode;
		}

		public MutationMode Mode { get; private set; }
		public bool Silent { get; set; }

		protected override BinaryRequest CreateRequest()
		{
			OpCode op;

			// figure out the op code
			if (Mode == MutationMode.Increment) op = OpCode.Increment;
			else if (Mode == MutationMode.Decrement) op = OpCode.Decrement;
			else throw new ArgumentOutOfRangeException("Unknown mode: " + Mode);

			// make it silent
			if (Silent) op = (OpCode)((byte)op | Protocol.SILENT_MASK);

			var request = new BinaryRequest(op, ExtraLength) { Key = Key, Cas = Cas };
			var extra = request.Extra.Array;
			var offset = request.Extra.Offset;

			// store the extra values
			BinaryConverter.EncodeUInt64(this.delta, extra, offset);
			BinaryConverter.EncodeUInt64(this.defaultValue, extra, offset + 8);
			BinaryConverter.EncodeUInt32(this.expires, extra, offset + 16);

			return request;
		}

		protected override IMutateOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new MutateOperationResult();

			if (response == null)
				return retval.NotFound(this);

			if (response.Success)
			{
				var data = response.Data;
				if (data.Count != ResultLength)
					return retval.Fail(this, new InvalidOperationException("Result must be " + ResultLength + " bytes long, received: " + data.Count));

				retval.Value = BinaryConverter.DecodeUInt64(data.Array, data.Offset);
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
