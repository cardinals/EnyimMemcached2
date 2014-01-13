using System;
using System.Collections.Generic;
using System.Text;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	/// <summary>
	/// Implements append/prepend.
	/// </summary>
	public class ConcatOperation : BinarySingleItemOperation<IOperationResult>, IConcatOperation
	{
		public ConcatOperation(ConcatenationMode mode, string key, ArraySegment<byte> data)
			: base(key)
		{
			Data = data;
			Mode = mode;
		}

		public ConcatenationMode Mode { get; private set; }
		public ArraySegment<byte> Data { get; private set; }
		public bool Silent { get; set; }

		protected override BinaryRequest CreateRequest()
		{
			OpCode op;

			switch (Mode)
			{
				case ConcatenationMode.Append: op = Silent ? OpCode.AppendQ : OpCode.Append; break;
				case ConcatenationMode.Prepend: op = Silent ? OpCode.PrependQ : OpCode.Prepend; break;
				default: throw new ArgumentOutOfRangeException("Unknown mode: " + Mode);
			}

			return new BinaryRequest(op)
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
