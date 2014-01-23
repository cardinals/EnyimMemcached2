using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Enyim.Caching.Memcached.Operations
{
	public class BinaryResponse : IResponse
	{
		private const int STATE_NEED_HEADER = 1;
		private const int STATE_NEED_BODY = 2;
		private const int STATE_DONE = 3;

		private string responseMessage;

		private byte[] header;
		private byte[] body;

		private int state;
		private int remainingHeader;
		private int remainingData;

		public BinaryResponse()
		{
			StatusCode = -1;
			header = new byte[Protocol.HeaderLength];
			remainingHeader = Protocol.HeaderLength;
			state = STATE_NEED_HEADER;
		}

		public byte OpCode;
		public int KeyLength;
		public byte DataType;
		public int StatusCode;

		public uint CorrelationId;
		public ulong CAS;

		public ArraySegment<byte> Extra;
		public ArraySegment<byte> Data;

		public bool Success { get { return StatusCode == Protocol.Status.Success; } }

		public string GetStatusMessage()
		{
			return this.Data.Array == null
					? null
					: (this.responseMessage ?? (this.responseMessage = Encoding.ASCII.GetString(this.Data.Array, this.Data.Offset, this.Data.Count)));
		}

		public bool Read(Stream stream)
		{
			if (state == STATE_NEED_HEADER)
			{
				remainingHeader -= stream.Read(header, Protocol.HeaderLength - remainingHeader, remainingHeader);
				Debug.Assert(remainingHeader >= 0);

				if (remainingHeader > 0)
					return true;

				if (!ProcessHeader(header, out remainingData))
				{
					state = STATE_DONE;
					return false;
				}

				state = STATE_NEED_BODY;
			}

			if (state == STATE_NEED_BODY)
			{
				remainingData -= stream.Read(body, body.Length - remainingData, remainingData);
				Debug.Assert(remainingData >= 0);

				if (remainingData > 0)
					return true;

				state = STATE_DONE;
			}

			return false;
		}

		/// <summary>
		/// Returns true if further IO is pending. (ie. body must be read)
		/// </summary>
		/// <param name="header"></param>
		/// <param name="bodyLength"></param>
		/// <param name="extraLength"></param>
		/// <returns></returns>
		private bool ProcessHeader(byte[] header, out int bodyLength)
		{
			if (header[Protocol.HEADER_INDEX_MAGIC] != Protocol.ResponseMagic)
				throw new InvalidOperationException("Expected magic value " + Protocol.ResponseMagic + ", received: " + header[Protocol.HEADER_INDEX_MAGIC]);

			OpCode = header[Protocol.HEADER_INDEX_OPCODE];
			KeyLength = BinaryConverter.DecodeUInt16(header, Protocol.HEADER_INDEX_KEY);
			var extraLength = header[Protocol.HEADER_INDEX_EXTRA];
			DataType = header[Protocol.HEADER_INDEX_DATATYPE];
			StatusCode = BinaryConverter.DecodeUInt16(header, Protocol.HEADER_INDEX_STATUS);
			bodyLength = BinaryConverter.DecodeInt32(header, Protocol.HEADER_INDEX_BODY);
			CorrelationId = unchecked((uint)BinaryConverter.DecodeInt32(header, Protocol.HEADER_INDEX_OPAQUE));
			CAS = BinaryConverter.DecodeUInt64(header, Protocol.HEADER_INDEX_CAS);

			if (bodyLength > 0)
			{
				body = new byte[bodyLength];

				Extra = new ArraySegment<byte>(body, 0, extraLength);
				Data = new ArraySegment<byte>(body, extraLength, bodyLength - extraLength);

				return true;
			}

			return false;
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
