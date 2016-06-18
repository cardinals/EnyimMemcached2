using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;

namespace Enyim.Caching
{
#if DIAGNOSTICS
	internal class BufferAllocatorPerformanceMonitor
	{
		private readonly IMeter allocCount;
		private readonly IMeter releaseCount;
		private readonly ICounter totalSize;

		public BufferAllocatorPerformanceMonitor(string name)
		{
			allocCount = Metrics.Meter("alloc count", name, Interval.Seconds);
			releaseCount = Metrics.Meter("alloc release count", name, Interval.Seconds);
			totalSize = Metrics.Counter("alloc total size", name);
		}

		public void Reset()
		{
			allocCount.Reset();
			releaseCount.Reset();
			totalSize.Reset();
		}

		public void Take(int size)
		{
			allocCount.IncrementBy(1);
			totalSize.IncrementBy(size);
		}

		public void Return(int size)
		{
			releaseCount.IncrementBy(1);
			totalSize.DecrementBy(size);
		}
	}
#else
	internal class BufferAllocatorPerformanceMonitor
	{
		public BufferAllocatorPerformanceMonitor(string name) { }

		[Conditional("DIAGNOSTICS")]
		public void Reset() { }

		[Conditional("DIAGNOSTICS")]
		public void Take(int size) { }

		[Conditional("DIAGNOSTICS")]
		public void Return(int size) { }
	}
#endif
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
