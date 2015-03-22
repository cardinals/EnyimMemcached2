using System;
using System.Collections.Generic;
using System.Linq;

namespace Enyim.Caching.Memcached.Operations
{
	public class GetAndTouchOperation : GetOperation, IGetAndTouchOperation
	{
		protected const int ExtraLength = 4;
		private const OpCode LoudOp = OpCode.GAT;
		private const OpCode SilentOp = (OpCode)((int)LoudOp | Protocol.SILENT_MASK);

		private OpCode operation = LoudOp;
		private bool silent;

		public GetAndTouchOperation(IBufferAllocator allocator, Key key) : base(allocator, key) { }

		public uint Expires { get; set; }

		public override bool Silent
		{
			get { return silent; }
			set
			{
				silent = value;
				operation = value ? SilentOp : LoudOp;
			}
		}

		protected override BinaryRequest CreateRequest()
		{
			var request = new BinaryRequest(Allocator, operation, ExtraLength)
			{
				Key = Key,
				Cas = Cas
			};

			// store expiration in Extra
			var extra = request.Extra;
			NetworkOrderConverter.EncodeUInt32(Expires, extra.Array, extra.Offset);

			return request;
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
