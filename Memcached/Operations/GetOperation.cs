﻿using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class GetOperation : BinarySingleItemOperation<IGetOperationResult>, IGetOperation
	{
		private const OpCode LoudOp = OpCode.Get;
		private const OpCode SilentOp = OpCode.GetQ;

		private OpCode operation = LoudOp;
		private bool silent;

		public GetOperation(IBufferAllocator allocator, Key key) : base(allocator, key) { }

		public virtual bool Silent
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
			return new BinaryRequest(Allocator, operation)
			{
				Key = Key,
				Cas = Cas
			};
		}

		protected override IGetOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new GetOperationResult();

			if (response == null)
				return retval.NotFound(this);

			if (response.StatusCode == 0)
			{
				var flags = NetworkOrderConverter.DecodeUInt32(response.Extra.Array, 0);

				retval.Value = new CacheItem((uint)flags, response.Data.Clone());
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
