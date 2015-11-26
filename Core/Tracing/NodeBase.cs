using System;
using System.Diagnostics.Tracing;

namespace Enyim.Caching
{
	public abstract partial class CoreEventSource
	{
		[Event(31, Message = "Write operation enqueued for {0}", Keywords = Keywords.OpQueue)]
		public abstract void EnqueueWriteOp(string address);
		[Event(32, Message = "Write operation dequeued for {0}", Keywords = Keywords.OpQueue)]
		public abstract void DequeueWriteOp(string address);

		[Event(33, Message = "Read operation enqueued for {0}", Keywords = Keywords.OpQueue)]
		public abstract void EnqueueReadOp(string address);
		[Event(34, Message = "Read operation dequeued for {0}", Keywords = Keywords.OpQueue)]
		public abstract void DequeueReadOp(string address);

		public static partial class Keywords
		{
			public const EventKeywords OpQueue = (EventKeywords)2;
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
