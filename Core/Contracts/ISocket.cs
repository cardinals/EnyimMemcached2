using System;
using System.Net;
using System.Threading;

namespace Enyim.Caching
{
	public interface ISocket : IDisposable
	{
		/// <summary>
		/// Gets a value that indicates whether the socket is still connected and can be used for communication.
		/// </summary>
		bool IsAlive { get; }

		/// <summary>
		/// Gets or sets a value that specifies the size of the send buffer.
		/// </summary>
		int SendBufferSize { get; set; }

		/// <summary>
		/// Gets or sets a value that specifies the size of the receive buffer.
		/// </summary>
		int ReceiveBufferSize { get; set; }

		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which a connect operation will time out and the socket is considered dead.
		/// </summary>
		TimeSpan ConnectionTimeout { get; set; }
		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which a send operation will time out and the socket is considered dead.
		/// </summary>
		TimeSpan SendTimeout { get; set; }
		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which a receive operation will time out and the socket is considered dead.
		/// </summary>
		TimeSpan ReceiveTimeout { get; set; }

		/// <summary>
		/// Gets the buffer that contains the data read from the remote endpoint.
		/// </summary>
		ReadBuffer ReadBuffer { get; }
		/// <summary>
		/// Gets the buffer that contains the data that will be sent to the remote endpoint.
		/// </summary>
		WriteBuffer WriteBuffer { get; }

		/// <summary>
		/// Establishes a connection to a remote host.
		/// </summary>
		/// <param name="endpoint">The endpoint to connect to.</param>
		/// <param name="token">cancel</param>
		void Connect(IPEndPoint endpoint, CancellationToken token);
		void ScheduleSend(Action<bool> whenDone);
		void ScheduleReceive(Action<bool> whenDone);

		//SocketState State { get; }
		void WaitOne(CancellationToken token);
	}

	public enum SocketState { Unspecified, Free, Connecting, Receiving, Sending }
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
