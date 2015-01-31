﻿using System;
using System.Text;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class TouchOperation : BinarySingleItemOperation<IOperationResult>, ITouchOperation
	{
		protected const int ExtraLength = 4;
		private static readonly Enyim.Caching.ILog log = Enyim.Caching.LogManager.GetLogger(typeof(TouchOperation));

		public TouchOperation(byte[] key, uint expires) :
			base(key)
		{
			Expires = expires;
		}

		public uint Expires { get; private set; }

		protected override BinaryRequest CreateRequest()
		{
			var request = new BinaryRequest(OpCode.Touch, ExtraLength)
			{
				Key = Key,
				Cas = Cas
			};

			// store expiration in Extra
			var extra = request.Extra;
			BinaryConverter.EncodeUInt32(Expires, extra.Array, extra.Offset);

			return request;
		}

		protected override IOperationResult CreateResult(BinaryResponse response)
		{
			var retval = new BinaryOperationResult();

			if (response == null)
				return retval.Success(this);

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