using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;

namespace Enyim.Caching
{
	public sealed class BufferManagerAllocator : IBufferAllocator
	{
		private readonly BufferManager pool;
		private readonly BufferAllocatorPerformanceMonitor perfMon;

		public BufferManagerAllocator(int maxBufferSize = 1024 * 1024, long maxBufferPoolSize = 1024 * 1024 * 16)
		{
			pool = BufferManager.CreateBufferManager(maxBufferPoolSize, maxBufferSize);
			perfMon = new BufferAllocatorPerformanceMonitor("BufferManagerAllocator");
		}

		public void Dispose()
		{
			pool.Clear();
		}

		public byte[] Take(int size)
		{
			var buffer = pool.TakeBuffer(size);
			Array.Clear(buffer, 0, buffer.Length);

			perfMon.Take(buffer.Length);

			return buffer;
		}

		public void Return(byte[] buffer)
		{
			perfMon.Return(buffer.Length);

			pool.ReturnBuffer(buffer);
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
