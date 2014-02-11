using System;
using System.Net;
using System.Threading;

namespace Enyim.Caching
{
	public interface ISocket : IDisposable
	{
		void Connect(IPEndPoint endpoint, CancellationToken token);
		int Receive(byte[] buffer, int offset, int count);
		void Send(byte[] buffer, int offset, int count);

		bool IsAlive { get; }
		int BufferSize { get; set; }
		TimeSpan ConnectionTimeout { get; set; }
		TimeSpan ReceiveTimeout { get; set; }

		void ReceiveAsync(byte[] buffer, int offset, int count, Action<int> whenDone);
		bool ReceiveInProgress { get; }
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
