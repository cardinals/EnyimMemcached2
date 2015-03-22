using System;
using System.Diagnostics;
using System.IO;

namespace Enyim.Caching
{
	public class ReadBuffer
	{
		private readonly byte[] readBuffer;
		private readonly int bufferOffset;
		private readonly int bufferLength;

		private int length;
		private int position;

		internal ReadBuffer(byte[] buffer, int offset, int count)
		{
			readBuffer = buffer;
			bufferOffset = offset;
			bufferLength = count;
		}

		public int Position { get { return position; } }
		public int Length { get { return length; } }
		public bool IsEmpty { get { return position == length; } }

		public int Read(byte[] buffer, int offset, int count)
		{
			var canRead = length - position;

			if (canRead <= 0) return 0;
			if (canRead > count) canRead = count;

			Buffer.BlockCopy(readBuffer, bufferOffset + position, buffer, offset, canRead);

			position += canRead;

			return canRead;
		}

		internal void SetAvailableLength(int length)
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
