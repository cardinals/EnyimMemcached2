using System;
using System.Text;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Operations
{
	public class StoreOperation : BinarySingleItemOperation<IOperationResult>, IStoreOperation
	{
		private static readonly Enyim.Caching.ILog log = Enyim.Caching.LogManager.GetLogger(typeof(StoreOperation));

		private CacheItem value;
		private uint expires;

		public StoreOperation(StoreMode mode, string key, CacheItem value, uint expires) :
			base(key)
		{
			Mode = mode;
			this.value = value;
			this.expires = expires;
		}

		public StoreMode Mode { get; private set; }
		public bool Silent { get; set; }

		protected override BinaryRequest CreateRequest()
		{
			OpCode op;
			switch (Mode)
			{
				case StoreMode.Add: op = Silent ? OpCode.AddQ : OpCode.Add; break;
				case StoreMode.Set: op = Silent ? OpCode.SetQ : OpCode.Set; break;
				case StoreMode.Replace: op = Silent ? OpCode.ReplaceQ : OpCode.Replace; break;
				default: throw new ArgumentOutOfRangeException("mode", Mode + " is not supported");
			}

			var extra = new byte[8];

			BinaryConverter.EncodeUInt32((uint)value.Flags, extra, 0);
			BinaryConverter.EncodeUInt32(expires, extra, 4);

			var request = new BinaryRequest(op)
			{
				Key = Key,
				Cas = Cas,
				Extra = new ArraySegment<byte>(extra),
				Data = value.Data
			};

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
