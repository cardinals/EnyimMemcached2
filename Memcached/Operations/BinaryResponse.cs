using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Enyim.Caching.Memcached.Operations
{
	public class BinaryResponse : IResponse
	{
		private const int STATE_NEED_HEADER = 0;
		private const int STATE_NEED_BODY = 1;
		private const int STATE_DONE = 2;

		private readonly IBufferAllocator allocator;
		private string responseMessage;
		private byte[] header;
		private byte[] data;

		private int state;
		private int remainingHeader;
		private int dataReadOffset;
		private int remainingData;

		internal BinaryResponse(IBufferAllocator allocator)
		{
			this.allocator = allocator;
			StatusCode = -1;
			header = allocator.Take(Protocol.HeaderLength);
			remainingHeader = Protocol.HeaderLength;
		}

		public byte OpCode { get; private set; }
		public int KeyLength { get; private set; }
		public byte DataType { get; private set; }
		public int StatusCode { get; private set; }

		public uint CorrelationId { get; private set; }
		public ulong CAS { get; private set; }

		public ArraySegment<byte> Extra { get; private set; }
		public ArraySegment<byte> Data { get; private set; }

		public bool Success { get { return StatusCode == Protocol.Status.Success; } }

		~BinaryResponse()
		{
			GC.WaitForPendingFinalizers();

			Dispose();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			if (header != null)
			{
				allocator.Return(header);
				header = null;
			}

			if (data != null)
			{
				allocator.Return(data);
				data = null;
			}
		}

		public string GetStatusMessage()
		{
			return Data.Array == null
					? null
					: (responseMessage ?? (responseMessage = Encoding.ASCII.GetString(Data.Array, Data.Offset, Data.Count)));
		}

		bool IResponse.Read(ReadBuffer buffer)
		{
			switch (state)
			{
				case STATE_NEED_HEADER:
					remainingHeader -= buffer.Read(header, Protocol.HeaderLength - remainingHeader, remainingHeader);
					if (remainingHeader > 0) return true;

					if (!ProcessHeader(header, out remainingData))
					{
						state = STATE_DONE;
						return false;
					}

					state = STATE_NEED_BODY;
					goto case STATE_NEED_BODY;

				case STATE_NEED_BODY:
					var read = buffer.Read(data, dataReadOffset, remainingData);
					remainingData -= read;
					dataReadOffset += read;

					if (remainingData > 0) return true;

					state = STATE_DONE;
					break;
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
				throw new InvalidOperationException($"Expected magic value {Protocol.ResponseMagic}, received: {header[Protocol.HEADER_INDEX_MAGIC]}");

			// TODO test if unsafe array gives a perf boost
			OpCode = header[Protocol.HEADER_INDEX_OPCODE];
			KeyLength = NetworkOrderConverter.DecodeUInt16(header, Protocol.HEADER_INDEX_KEY);
			DataType = header[Protocol.HEADER_INDEX_DATATYPE];
			StatusCode = NetworkOrderConverter.DecodeUInt16(header, Protocol.HEADER_INDEX_STATUS);
			CorrelationId = NetworkOrderConverter.DecodeUInt32(header, Protocol.HEADER_INDEX_OPAQUE);
			CAS = NetworkOrderConverter.DecodeUInt64(header, Protocol.HEADER_INDEX_CAS);

			bodyLength = (int)NetworkOrderConverter.DecodeUInt32(header, Protocol.HEADER_INDEX_BODY);

			if (bodyLength > 0)
			{
				var extraLength = header[Protocol.HEADER_INDEX_EXTRA];

				data = allocator.Take(bodyLength);

				Extra = new ArraySegment<byte>(data, 0, extraLength);
				Data = new ArraySegment<byte>(data, extraLength, bodyLength - extraLength);

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
