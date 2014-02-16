using System;
using System.Net;
using System.Threading;

namespace Enyim.Caching
{
	public interface ISocket : IDisposable
	{
		bool IsAlive { get; }
		ReadBuffer ReadBuffer { get; }
		WriteBuffer WriteBuffer { get; }

		TimeSpan ConnectionTimeout { get; set; }
		TimeSpan ReceiveTimeout { get; set; }

		void Connect(IPEndPoint endpoint, CancellationToken token);
		void ScheduleReceive(Action<bool> whenDone);
		void ScheduleSend(Action<bool> whenDone);
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
