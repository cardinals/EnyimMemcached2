using System;
using System.Diagnostics;
using System.IO;

namespace Enyim.Caching
{
	internal class ReceiveBuffer : Stream
	{
		private readonly byte[] readBuffer;

		private int length;
		private int position;

		public ReceiveBuffer(int bufferSize)
		{
			readBuffer = new byte[bufferSize];
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var canRead = length - position;

			if (canRead <= 0) return 0;
			if (canRead > count) canRead = count;

			Buffer.BlockCopy(readBuffer, position, buffer, offset, canRead);

			position += canRead;

			return canRead;
		}

		public void Fill(ISocket socket)
		{
			Debug.Assert(!socket.ReceiveInProgress);

			length = socket.Receive(readBuffer, 0, readBuffer.Length);
			position = 0;
		}

		public void FillAsync(ISocket socket, Action whenDone)
		{
			Debug.Assert(!socket.ReceiveInProgress);

			socket.ReceiveAsync(readBuffer, 0, readBuffer.Length, (read) =>
			{
				position = 0;
				length = read;
				whenDone();
			});
		}

		public void Reset()
		{
			position = 0;
			length = 0;
		}

		public bool EOF
		{
			get { return position == length; }
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get { return length; }
		}

		public override long Position
		{
			get { return position; }
			set { throw new NotSupportedException(); }
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
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
