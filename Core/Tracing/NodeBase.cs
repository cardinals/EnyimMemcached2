using System;
using System.Diagnostics.Tracing;

namespace Enyim.Caching
{
	public static partial class CoreEventSource
	{
		private const int NodeBaseTaskId = 20;

		[Event(NodeBaseTaskId + 1, Message = "Write operation enqueued for {0}", Keywords = Keywords.OpQueue)]
		public static void EnqueueWriteOp(string address) { }
		[Event(NodeBaseTaskId + 2, Message = "Write operation dequeued for {0}", Keywords = Keywords.OpQueue)]
		public static void DequeueWriteOp(string address) { }

		[Event(NodeBaseTaskId + 3, Message = "Read operation enqueued for {0}", Keywords = Keywords.OpQueue)]
		public static void EnqueueReadOp(string address) { }
		[Event(NodeBaseTaskId + 4, Message = "Read operation dequeued for {0}", Keywords = Keywords.OpQueue)]
		public static void DequeueReadOp(string address) { }

		[Event(NodeBaseTaskId + 5, Message = "Error while enqueueing for {0}", Keywords = Keywords.OpQueue)]
		public static void NodeError(string address) { }

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
