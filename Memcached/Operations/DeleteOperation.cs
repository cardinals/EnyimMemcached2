using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class DeleteOperation : BinarySingleItemOperation<IOperationResult>, IDeleteOperation
	{
		private const OpCode LoudOp = OpCode.Delete;
		private const OpCode SilentOp = OpCode.DeleteQ;

		private OpCode operation = LoudOp;
		private bool silent;

		public DeleteOperation(IBufferAllocator allocator, Key key) : base(allocator, key) { }

		public bool Silent
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
			var request = new BinaryRequest(Allocator, operation)
			{
				Key = Key,
				Cas = Cas
			};

			return request;
		}

		protected override IOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new BinaryOperationResult();

			return (response == null)
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
