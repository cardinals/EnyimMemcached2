using System;
using System.Diagnostics.Tracing;
using System.Net.Sockets;

namespace Enyim.Caching
{
	partial class CoreEventSource
	{
		private const int AsyncSocketTaskId = 0;

		[Event(AsyncSocketTaskId + 1, Level = EventLevel.Informational, Keywords = Keywords.Socket)]
		public static void ConnectStart(string endpoint) { }
		[Event(AsyncSocketTaskId + 2, Level = EventLevel.Informational, Keywords = Keywords.Socket)]
		public static void ConnectStop(string endpoint) { }

		[Event(AsyncSocketTaskId + 3, Level = EventLevel.Error, Keywords = Keywords.Socket)]
		public static void ConnectFail(string endpoint, SocketError status) { }

		[Event(AsyncSocketTaskId + 4, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public static void SendStart(string endpoint, bool isAlive, int byteCount) { }
		[Event(AsyncSocketTaskId + 5, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public static void SendStop(string endpoint, bool isAlive, bool success) { }
		[Event(AsyncSocketTaskId + 6, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public static void SendChunk(string endpoint, bool isAlive, int bytesSent, SocketError status) { }

		[Event(AsyncSocketTaskId + 7, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public static void ReceiveStart(string endpoint, bool isAlive) { }
		[Event(AsyncSocketTaskId + 8, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public static void ReceiveStop(string endpoint, bool isAlive, bool success) { }
		[Event(AsyncSocketTaskId + 9, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public static void ReceiveChunk(string endpoint, bool isAlive, int bytesReceived, SocketError status) { }

		public static partial class Keywords
		{
			public const EventKeywords Socket = (EventKeywords)1;
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
