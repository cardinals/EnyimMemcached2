using System;

namespace Enyim.Caching
{
	public class WriteBuffer
	{
		private readonly byte[] buffer;
		private readonly int bufferOffset;
		private readonly int length;
		private int position;

		internal WriteBuffer(byte[] buffer, int offset, int count)
		{
			this.buffer = buffer;
			this.bufferOffset = offset;
			this.length = count;
		}

		internal int BufferOffset { get { return bufferOffset; } }
		public int Length { get { return length; } }
		public int Position { get { return position; } }
		public bool IsFull { get { return position == length; } }

		public int Append(byte[] data, int offset, int count)
		{
			var canWrite = length - position;

			if (canWrite <= 0) return 0;
			if (canWrite > count) canWrite = count;

			Buffer.BlockCopy(data, offset, buffer, position + bufferOffset, canWrite);

			position += canWrite;

			return canWrite;
		}

		public bool CanWrite(int count)
		{
			return length - position >= count;
		}

		internal void Reset()
		{
			position = 0;
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
