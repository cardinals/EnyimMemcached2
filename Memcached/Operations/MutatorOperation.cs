using System;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class MutateOperation : BinarySingleItemOperation<IMutateOperationResult>, IMutateOperation
	{
		private ulong defaultValue;
		private ulong delta;
		private uint expires;

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

		protected override BinaryRequest CreateRequest()
		{
			OpCode op;

			switch (Mode)
			{
				case MutationMode.Increment: op = OpCode.IncrementQ; break;
				case MutationMode.Decrement: op = OpCode.DecrementQ; break;
				default: throw new ArgumentOutOfRangeException("Unknown mode: " + Mode);
			}

			var request = new BinaryRequest(op)
			{
				Key = Key,
				Cas = Cas,
				Extra = CreateExtra()
			};

			return request;
		}

		protected ArraySegment<byte> CreateExtra()
		{
			var extra = new byte[20];

			BinaryConverter.EncodeUInt64(this.delta, extra, 0);
			BinaryConverter.EncodeUInt64(this.defaultValue, extra, 8);
			BinaryConverter.EncodeUInt32(this.expires, extra, 16);

			return new ArraySegment<byte>(extra);
		}

		protected override IMutateOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new MutateOperationResult();

			if (response == null)
				return retval.NotFound(this);

			if (response.StatusCode == 0)
			{
				var data = response.Data;
				if (data.Count != 8)
					return retval.Fail(this, new InvalidOperationException("Result must be 8 bytes long, received: " + data.Count));

				retval.Value = BinaryConverter.DecodeUInt64(data.Array, data.Offset);
			}

			return retval.WithResponse(response);
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
