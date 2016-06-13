using System;
using System.Diagnostics;

namespace Enyim.Caching
{
	public sealed class WriteBuffer
	{
		private readonly byte[] writeBuffer;
		private readonly int capacity;
		private int position;

		internal readonly int BufferOffset;

		internal WriteBuffer(byte[] buffer, int offset, int capacity)
		{
			this.writeBuffer = buffer;
			this.BufferOffset = offset;
			this.capacity = capacity;
		}

		/// <summary>
		/// The size of the buffer.
		/// </summary>
		public int Capacity { get { return capacity; } }

		/// <summary>
		/// The current position in the buffer
		/// </summary>
		public int Position { get { return position; } }
		public bool IsFull { get { return position == capacity; } }

		public int Append(byte[] data, int offset, int count)
		{
			var toWrite = capacity - position;

			if (toWrite <= 0) return 0;
			if (toWrite > count) toWrite = count;

			System.Buffer.BlockCopy(data, offset, writeBuffer, BufferOffset + position, toWrite);

			position += toWrite;
			Debug.Assert(position <= capacity);

			return toWrite;
		}

		internal void Clear()
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
