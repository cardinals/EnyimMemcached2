using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Enyim.Caching.Memcached.Operations
{
	public class BinaryRequest : IRequest
	{
		// 128 is the minimum size handled by the BuffferManager
		private const int ArraySize = 128;

		private const int STATE_INITIAL = 0;
		private const int STATE_WRITE_HEADER = 1;
		private const int STATE_WRITE_KEY = 2;
		private const int STATE_WRITE_BODY = 3;
		private const int STATE_DONE = 4;

		private static readonly BufferPool bufferPool = new BufferPool(ArraySize, ArraySize * 1024);

		private static int InstanceCounter;

		private readonly int headerLength;
		private byte[] header;

		private int state;
		private int writeOffset;

		public BinaryRequest(OpCode operation) : this((byte)operation, 0) { }
		public BinaryRequest(OpCode operation, byte extraLength) : this((byte)operation, extraLength) { }

		public BinaryRequest(byte commandCode, byte extraLength)
		{
			this.Operation = commandCode;
			this.CorrelationId = unchecked((uint)Interlocked.Increment(ref InstanceCounter)); // request id

			this.headerLength = Protocol.HeaderLength + extraLength;

			// prealloc header so that the extra data can be placed into the same buffer
			this.header = bufferPool.Acquire(headerLength);
			this.Extra = extraLength == 0
							? new ArraySegment<byte>()
							: new ArraySegment<byte>(header, Protocol.HeaderLength, extraLength);
		}

		~BinaryRequest()
		{
			GC.WaitForPendingFinalizers();
			Dispose();
		}

		public void Dispose()
		{
			if (header != null)
			{
				GC.SuppressFinalize(this);
				bufferPool.Release(header);
				header = null;
			}
		}

		bool IRequest.WriteTo(WriteBuffer buffer)
		{
			// 0. init header
			// 1. loop on header
			// 2. loop on Key, if any
			// 3. loop on Data, if any
			// 4. done

			switch (state)
			{
				case STATE_INITIAL: goto init;
				case STATE_WRITE_HEADER: goto write_header;
				case STATE_WRITE_KEY: goto write_key;
				case STATE_WRITE_BODY: goto write_body;
				default: return false;
			}

		init:
			PrepareHeader();
			state = STATE_WRITE_HEADER;

		write_header:
			writeOffset += buffer.Append(header, writeOffset, headerLength - writeOffset);
			if (writeOffset < headerLength) return true;

			if (Key != null && Key.Length > 0)
			{
				writeOffset = 0;
				state = STATE_WRITE_KEY;
			}
			else goto pre_body;

		write_key:
			writeOffset += buffer.Append(Key, writeOffset, Key.Length - writeOffset);
			if (writeOffset < Key.Length) return true;

		pre_body:
			if (Data.Count > 0)
			{
				writeOffset = 0;
				state = STATE_WRITE_BODY;
			}
			else goto done;

		write_body:
			writeOffset += buffer.Append(Data.Array, Data.Offset + writeOffset, Data.Count - writeOffset);
			if (writeOffset < Data.Count) return true;

		done:
			state = STATE_DONE;

			return false;
		}

		private void PrepareHeader()
		{
			var keyLength = Key == null ? 0 : Key.Length;
			if (keyLength > Protocol.MaxKeyLength) throw new InvalidOperationException("KeyTooLong");

			var totalLength = Extra.Count + keyLength + Data.Count;	// total payload size
			var header = this.header;

			header[Protocol.HEADER_INDEX_MAGIC] = Protocol.RequestMagic; // magic
			header[Protocol.HEADER_INDEX_OPCODE] = Operation;

			header[Protocol.HEADER_INDEX_KEY + 0] = (byte)(keyLength >> 8);
			header[Protocol.HEADER_INDEX_KEY + 1] = (byte)(keyLength & 255);
			header[Protocol.HEADER_INDEX_EXTRA] = (byte)(Extra.Count);

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

			var cas = Cas; // skip this if no cas is specified
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
		}

		public readonly byte Operation;
		public readonly uint CorrelationId;
		public byte[] Key;
		public ulong Cas;
		//public ushort Reserved;
		public readonly ArraySegment<byte> Extra;
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
