using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Enyim.Caching.Memcached.Operations
{
	public sealed class BinaryResponse : IResponse
	{
		private const int STATE_NEED_HEADER = 0;
		private const int STATE_SETUP_EXTRA = 1;
		private const int STATE_READ_EXTRA = 2;
		private const int STATE_SETUP_BODY = 3;
		private const int STATE_READ_BODY = 4;
		private const int STATE_DONE = 5;

		private readonly IBufferAllocator allocator;
		private string responseMessage;
		private byte[] header;

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

		public byte OpCode;
		public int KeyLength;
		public byte DataType;
		public int StatusCode;

		public uint CorrelationId;
		public ulong CAS;

		public ByteBuffer Extra;
		public ByteBuffer Data;

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

			Extra.Dispose();
			Data.Dispose();
		}

		public string GetStatusMessage()
		{
			return Data.Array == null
					? null
					: (responseMessage ?? (responseMessage = Encoding.ASCII.GetString(Data.Array, 0, Data.Length)));
		}

		bool IResponse.Read(ReadBuffer buffer)
		{
			int read;

			switch (state)
			{
				case STATE_NEED_HEADER:
					remainingHeader -= buffer.Read(header, Protocol.HeaderLength - remainingHeader, remainingHeader);

					if (remainingHeader > 0) return true;
					if (!ProcessHeader(header)) goto case STATE_DONE;

					goto case STATE_SETUP_EXTRA;

				case STATE_SETUP_EXTRA:
					if (Extra.Length == 0) goto case STATE_SETUP_BODY;

					dataReadOffset = 0;
					remainingData = Extra.Length;

					state = STATE_READ_EXTRA;
					goto case STATE_READ_EXTRA;

				case STATE_READ_EXTRA:
					read = buffer.Read(Extra.Array, dataReadOffset, remainingData);
					dataReadOffset += read;

					remainingData -= read;
					if (remainingData > 0) return true;

					goto case STATE_SETUP_BODY;

				case STATE_SETUP_BODY:
					if (Data.Length == 0) goto case STATE_DONE;

					dataReadOffset = 0;
					remainingData = Data.Length;

					state = STATE_READ_BODY;
					goto case STATE_READ_BODY;

				case STATE_READ_BODY:
					read = buffer.Read(Data.Array, dataReadOffset, remainingData);
					dataReadOffset += read;

					remainingData -= read;
					if (remainingData > 0) return true;

					goto case STATE_DONE;

				case STATE_DONE:
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
		private bool ProcessHeader(byte[] header)
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

			var bodyLength = (int)NetworkOrderConverter.DecodeUInt32(header, Protocol.HEADER_INDEX_BODY_LENGTH);

			if (bodyLength > 0)
			{
				var extraLength = header[Protocol.HEADER_INDEX_EXTRA];
				if (extraLength > 0)
					Extra = ByteBuffer.Allocate(allocator, extraLength);

				var dataLength = bodyLength - extraLength;
				if (dataLength > 0)
					Data = ByteBuffer.Allocate(allocator, dataLength);

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
