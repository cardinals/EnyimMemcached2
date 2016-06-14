using System;
using System.Diagnostics;
using System.IO;

namespace Enyim.Caching
{
	public sealed class ReadBuffer
	{
		private readonly byte[] readBuffer;
		private readonly int bufferLength;

		private int length;
		private int position;

		internal readonly int BufferOffset;

		internal ReadBuffer(byte[] buffer, int offset, int count)
		{
			readBuffer = buffer;
			BufferOffset = offset;
			bufferLength = count;
		}

		public int Length { get { return length; } }
		public bool IsEmpty { get { return position == length; } }

		/// <summary>
		/// Reads a sequence of bytes
		/// </summary>
		/// <param name="buffer">
		///    An array of bytes. When this method returns, the buffer contains the specified
		///    byte array with the values between offset and (offset + count - 1) replaced by
		///    the bytes read from the current source.</param>
		/// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
		/// <param name="count">The maximum number of bytes to be read.</param>
		/// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
		public int Read(byte[] buffer, int offset, int count)
		{
			var toRead = length - position;

			if (toRead <= 0) return 0;
			if (toRead > count) toRead = count;

			Buffer.BlockCopy(readBuffer, BufferOffset + position, buffer, offset, toRead);

			position += toRead;

			return toRead;
		}

		internal void SetDataLength(int length)
		{
			Debug.Assert(length <= bufferLength, "length cannot be larger than bufferLength");

			this.position = 0;
			this.length = length;
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
