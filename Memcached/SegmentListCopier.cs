using System;
using System.Collections.Generic;

namespace Enyim.Caching.Memcached
{
	internal class SegmentListCopier
	{
		private IReadOnlyList<ArraySegment<byte>> segments;
		private int segmentIndex;
		private int segmentOffset;

		public SegmentListCopier(IReadOnlyList<ArraySegment<byte>> segments)
		{
			var length = 0;
			for (var i = 0; i < segments.Count; i++)
				length += segments[i].Count;

			this.Length = length;
			this.segments = segments;
		}

		public int Length { get; private set; }

		/// <summary>
		/// Returns true if IO is pending
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		public bool WriteTo(WriteBuffer buffer)
		{
			while (!buffer.IsFull && segments != null)
			{
				var current = segments[segmentIndex];

				if (current.Count > 0)
					segmentOffset += buffer.Write(current.Array, current.Offset + segmentOffset, current.Count - segmentOffset);

				if (segmentOffset == current.Count)
					MoveToNext();
			}

			return segments != null;
		}

		private void MoveToNext()
		{
			segmentOffset = 0;
			segmentIndex++;

			if (segmentIndex == segments.Count)
				segments = null;
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
