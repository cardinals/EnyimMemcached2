using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Threading;

namespace Enyim.Caching.Memcached.Operations
{
	public class BinaryRequest : IRequest
	{
		private static int InstanceCounter;

		private byte[] header;

		public BinaryRequest(OpCode operation) : this((byte)operation, 0) { }
		public BinaryRequest(OpCode operation, byte extraLength) : this((byte)operation, extraLength) { }

		public BinaryRequest(byte commandCode, byte extraLength)
		{
			this.Operation = commandCode;
			this.CorrelationId = unchecked((uint)Interlocked.Increment(ref InstanceCounter)); // session id

			this.header = new byte[Protocol.HeaderLength + extraLength];
			this.Extra = extraLength == 0
							? new ArraySegment<byte>()
							: new ArraySegment<byte>(header, Protocol.HeaderLength, extraLength);
		}

		public IReadOnlyList<ArraySegment<byte>> CreateBuffer()
		{
			var keyLength = Key == null ? 0 : Key.Length;
			if (keyLength > Protocol.MaxKeyLength) throw new InvalidOperationException("KeyTooLong");

			//var extra = Extra;
			//var extraLength = extra.Array == null ? 0 : extra.Count;
			//if (extraLength > Protocol.MaxExtraLength) throw new InvalidOperationException("ExtraTooLong");

			var body = Data;
			var bodyLength = body.Array == null ? 0 : body.Count; // body size
			var totalLength = Extra.Count + keyLength + bodyLength; // total payload size

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

			//if (extraLength > 0) retval.Add(extra);
			if (keyLength > 0) retval.Add(new ArraySegment<byte>(Key));
			if (bodyLength > 0) retval.Add(body);

			return retval;
		}

		public readonly byte Operation;
		public readonly uint CorrelationId;
		public byte[] Key;
		public ulong Cas;
		//public ushort Reserved;
		public readonly ArraySegment<byte> Extra;
		public ArraySegment<byte> Data;
	}





	public class BufferPool : IDisposable
	{
		private readonly BufferManager pool;

		public BufferPool(long maxBufferPoolSize, int maxBufferSize)
		{
			pool = BufferManager.CreateBufferManager(maxBufferPoolSize, maxBufferSize);
		}

		public void Dispose()
		{
			pool.Clear();
		}

		public byte[] TakeBuffer(int size)
		{
			var buffer = pool.TakeBuffer(size);
#if DEBUG
			trackedBuffers.GetOrCreateValue(buffer).Remember();
#endif
			return buffer;
		}

		public void ReturnBuffer(byte[] buffer)
		{
#if DEBUG
			Tracker value;
			if (trackedBuffers.TryGetValue(buffer, out value))
				value.Forget();
#endif
			pool.ReturnBuffer(buffer);
		}

		#region Leak detection

#if DEBUG

		private ConditionalWeakTable<byte[], Tracker> trackedBuffers = new ConditionalWeakTable<byte[], Tracker>();

		public class Tracker
		{
			private StackTrace stackTrace;

			public void Remember()
			{
				ThrowIfLeaking();

				stackTrace = new StackTrace(true);
			}

			public void Forget()
			{
				stackTrace = null;
			}

			private void ThrowIfLeaking()
			{
				if (stackTrace != null)
					throw new InvalidOperationException("Buffer leak: " + stackTrace);
			}
		}
#endif
		#endregion
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
