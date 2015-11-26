using System;
using System.Diagnostics.Tracing;
using System.Net.Sockets;

namespace Enyim.Caching
{
	public abstract partial class CoreEventSource : EventSource
	{
		[Event(1, Level = EventLevel.Informational, Keywords = Keywords.Socket)]
		public abstract void ConnectStart(string endpoint);
		[Event(2, Level = EventLevel.Informational, Keywords = Keywords.Socket)]
		public abstract void ConnectStop(string endpoint);

		[Event(3, Level = EventLevel.Error, Keywords = Keywords.Socket)]
		public abstract void ConnectFail(string endpoint, SocketError status);

		[Event(10, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public abstract void SendStart(string endpoint, bool isAlive, int byteCount);
		[Event(11, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public abstract void SendStop(string endpoint, bool isAlive, bool success);

		[Event(12, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public unsafe void SendChunk(string endpoint, bool isAlive, int bytesSent, SocketError status)
		{
			if (IsEnabled())
			{
				fixed (char* pEndpoint = endpoint)
				{
					var data = stackalloc EventData[4];

					data[0].DataPointer = (IntPtr)(pEndpoint);
					data[0].Size = (endpoint.Length + 1) * 2;

					var alive = isAlive ? 1 : 0;
					data[1].DataPointer = (IntPtr)(&alive);
					data[1].Size = sizeof(int);

					data[2].DataPointer = (IntPtr)(&bytesSent);
					data[2].Size = sizeof(int);

					data[3].Size = sizeof(int);
					data[3].DataPointer = (IntPtr)(&status);

					WriteEventCore(12, 4, data);
				}
			}
		}

		[Event(20, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public abstract void ReceiveStart(string endpoint, bool isAlive);
		[Event(21, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public abstract void ReceiveStop(string endpoint, bool isAlive, bool success);

		[Event(22, Level = EventLevel.Verbose, Keywords = Keywords.Socket)]
		public unsafe void ReceiveChunk(string endpoint, bool isAlive, int bytesReceived, SocketError status)
		{
			if (IsEnabled())
			{
				fixed (char* pEndpoint = endpoint)
				{
					var data = stackalloc EventData[4];

					data[0].DataPointer = (IntPtr)(pEndpoint);
					data[0].Size = (endpoint.Length + 1) * 2;

					var alive = isAlive ? 1 : 0;
					data[1].DataPointer = (IntPtr)(&alive);
					data[1].Size = sizeof(int);

					data[2].DataPointer = (IntPtr)(&bytesReceived);
					data[2].Size = sizeof(int);

					data[3].Size = sizeof(int);
					data[3].DataPointer = (IntPtr)(&status);

					WriteEventCore(22, 4, data);
				}
			}
		}

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
