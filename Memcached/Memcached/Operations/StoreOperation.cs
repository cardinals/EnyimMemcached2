using System;
using System.Text;

namespace Enyim.Caching.Memcached.Operations
{
	public class StoreOperation : BinarySingleItemOperation
	{
		private StoreMode mode;
		private uint expires;
		private byte[] value;

		public StoreOperation(StoreMode mode, string key, byte[] value, uint expires) :
			base(key)
		{
			this.mode = mode;
			this.value = value;
			this.expires = expires;
		}

		protected override BinaryRequest DoGetRequest()
		{
			OpCode op;
			switch (this.mode)
			{
				case StoreMode.Add: op = OpCode.AddQ; break;
				case StoreMode.Set: op = OpCode.SetQ; break;
				case StoreMode.Replace: op = OpCode.ReplaceQ; break;
				default: throw new ArgumentOutOfRangeException("mode", mode + " is not supported");
			}

			var extra = new byte[8];

			//BinaryConverter.EncodeUInt32((uint)this.value.Flags, extra, 0);
			BinaryConverter.EncodeUInt32(expires, extra, 4);

			var request = new BinaryRequest(op)
			{
				Key = this.Key,
				//Cas = this.Cas,
				Extra = new ArraySegment<byte>(extra),
				Data = new ArraySegment<byte>(this.value)
			};

			return request;
		}

		protected override void DoProcessResponse(BinaryResponse response)
		{
			if (response != null) StatusCode = response.StatusCode;
		}

		public override string ToString()
		{
			return mode + ": " + Key;
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
