using System;

namespace Enyim.Caching.Memcached
{
	public class WriteBuffer
	{
		private int capacity;
		private int position;
		private readonly byte[] writeBuffer;

		public WriteBuffer(int capacity)
		{
			this.capacity = capacity;
			this.writeBuffer = new byte[capacity];
		}

		public int Capacity { get { return capacity; } }
		public int Position { get { return position; } }
		public int Remaining { get { return capacity - position; } }
		public bool IsFull { get { return capacity == position; } }

		public int Write(ArraySegment<byte> buffer)
		{
			return Write(buffer.Array, buffer.Offset, buffer.Count);
		}

		public int Write(byte[] buffer, int offset, int count)
		{
			var canWrite = capacity - position;

			if (canWrite <= 0) return 0;
			if (canWrite > count) canWrite = count;

			Buffer.BlockCopy(buffer, offset, writeBuffer, position, canWrite);

			position += canWrite;

			return canWrite;
		}

		public byte[] GetBuffer()
		{
			return writeBuffer;
		}

		public void Reset()
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
