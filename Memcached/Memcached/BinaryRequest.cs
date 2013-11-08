using System;
using System.Collections.Generic;
using System.Threading;

namespace Enyim.Caching.Memcached.Operations
{
	public class BinaryRequest
	{
		private static int InstanceCounter;

		public BinaryRequest(OpCode operation) : this((byte)operation) { }

		public BinaryRequest(byte commandCode)
		{
			this.Operation = commandCode;
			this.CorrelationId = unchecked((uint)Interlocked.Increment(ref InstanceCounter)); // session id
		}

		public unsafe IReadOnlyList<ArraySegment<byte>> CreateBuffer()
		{
			var key = BinaryConverter.EncodeKey(Key);
			var keyLength = key == null ? 0 : key.Length;
			if (keyLength > Protocol.MaxKeyLength) throw new InvalidOperationException("KeyTooLong");

			var extra = Extra;
			var extraLength = extra.Array == null ? 0 : extra.Count;
			if (extraLength > Protocol.MaxExtraLength) throw new InvalidOperationException("ExtraTooLong");

			var body = Data;
			var bodyLength = body.Array == null ? 0 : body.Count; // body size
			var totalLength = extraLength + keyLength + bodyLength; // total payload size
			var header = new byte[Protocol.HeaderLength]; //build the header

			header[Protocol.HEADER_INDEX_MAGIC] = Protocol.RequestMagic; // magic
			header[Protocol.HEADER_INDEX_OPCODE] = Operation;

			header[Protocol.HEADER_INDEX_KEY + 0] = (byte)(keyLength >> 8);
			header[Protocol.HEADER_INDEX_KEY + 1] = (byte)(keyLength & 255);
			header[Protocol.HEADER_INDEX_EXTRA] = (byte)(extraLength);

			// 5 -- data type, 0 (RAW)
			// 6,7 -- reserved, always 0
			//buffer[0x05] = 0;
			//buffer[0x06] = (byte)(Reserved >> 8);
			//buffer[0x07] = (byte)(Reserved & 255);

			header[Protocol.HEADER_INDEX_BODY + 0] = (byte)(totalLength >> 24);
			header[Protocol.HEADER_INDEX_BODY + 1] = (byte)(totalLength >> 16);
			header[Protocol.HEADER_INDEX_BODY + 2] = (byte)(totalLength >> 8);
			header[Protocol.HEADER_INDEX_BODY + 3] = (byte)(totalLength & 255);

			var cid = CorrelationId;
			header[Protocol.HEADER_INDEX_OPAQUE + 0] = (byte)(cid >> 24);
			header[Protocol.HEADER_INDEX_OPAQUE + 1] = (byte)(cid >> 16);
			header[Protocol.HEADER_INDEX_OPAQUE + 2] = (byte)(cid >> 8);
			header[Protocol.HEADER_INDEX_OPAQUE + 3] = (byte)(cid & 255);

			var cas = Cas; // skip this if no cas is specfied
			if (cas > 0)
			{
				header[Protocol.HEADER_INDEX_CAS + 0] = (byte)(cas >> 56);
				header[Protocol.HEADER_INDEX_CAS + 1] = (byte)(cas >> 48);
				header[Protocol.HEADER_INDEX_CAS + 2] = (byte)(cas >> 40);
				header[Protocol.HEADER_INDEX_CAS + 3] = (byte)(cas >> 32);
				header[Protocol.HEADER_INDEX_CAS + 4] = (byte)(cas >> 24);
				header[Protocol.HEADER_INDEX_CAS + 5] = (byte)(cas >> 16);
				header[Protocol.HEADER_INDEX_CAS + 6] = (byte)(cas >> 8);
				header[Protocol.HEADER_INDEX_CAS + 7] = (byte)(cas & 255);
			}

			var retval = new List<ArraySegment<byte>>(4) { new ArraySegment<byte>(header) };

			if (extraLength > 0) retval.Add(extra);
			if (keyLength > 0) retval.Add(new ArraySegment<byte>(key));
			if (bodyLength > 0) retval.Add(body);

			return retval;
		}

		public readonly byte Operation;
		public readonly uint CorrelationId;
		public string Key;
		public ulong Cas;
		//public ushort Reserved;
		public ArraySegment<byte> Extra;
		public ArraySegment<byte> Data;
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
