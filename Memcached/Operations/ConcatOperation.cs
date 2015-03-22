using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	/// <summary>
	/// Implements append/prepend.
	/// </summary>
	public class ConcatOperation : BinarySingleItemOperation<IOperationResult>, IConcatOperation
	{
		private static readonly OpCode[] SilentOps = { OpCode.AppendQ, OpCode.PrependQ };
		private static readonly OpCode[] LoudOps = { OpCode.Append, OpCode.Prepend };

		private OpCode[] operations = LoudOps;
		private bool silent;

		public ConcatOperation(IBufferAllocator allocator, ConcatenationMode mode, Key key)
			: base(allocator, key)
		{
			Mode = mode;
		}

		public ConcatenationMode Mode { get; private set; }
		public ArraySegment<byte> Data { get; set; }

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
			return new BinaryRequest(Allocator, operations[(int)Mode])
			{
				Key = Key,
				Cas = Cas,
				Data = Data
			};
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
