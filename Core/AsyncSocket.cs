using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Enyim.Caching
{
	[DebuggerDisplay("[ Address: {endpoint}, IsAlive = {IsAlive} ]")]
	public class AsyncSocket : ISocket
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private static readonly bool LogTraceEnabled = log.IsTraceEnabled;
		private static readonly bool LogDebugEnabled = log.IsDebugEnabled;

		private readonly object InstanceLock = new object();

		private IPEndPoint endpoint;
		private int bufferSize;
		private TimeSpan receiveTimeout;
		private TimeSpan connectionTimeout;

		private Socket socket;
		private int state;

		private SocketAsyncEventArgs recvArgs;
		private ReadBuffer recvBuffer;

		private SocketAsyncEventArgs sendArgs;
		private WriteBuffer sendBuffer;

		public AsyncSocket()
		{
			ReceiveTimeout = TimeSpan.FromSeconds(10);
			ConnectionTimeout = TimeSpan.FromSeconds(10);
			BufferSize = CONSTS.BufferSize;
		}

		#region [ Cleanup                      ]

		~AsyncSocket()
		{
			try { this.Dispose(); }
			catch { }
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			sendArgs.Completed -= SendAsyncCompleted;
			recvArgs.Completed -= RecvAsyncCompleted;

			SocketAsyncEventArgsPool.Instance.Return(recvArgs);
			SocketAsyncEventArgsPool.Instance.Return(sendArgs);

			DestroySocket();
		}

		private void DestroySocket()
		{
			if (LogTraceEnabled) log.Trace("DestroySocket");

			if (socket != null)
			{
				try
				{
					using (socket)
					{
						socket.Shutdown(SocketShutdown.Both);
						socket.Close();
					}
				}
				catch (Exception e)
				{
					if (LogDebugEnabled) log.Debug("Exception while destroying socket.", e);
				}

				socket = null;
			}
		}

		#endregion
		#region [ Init                         ]

		private void InitBuffers()
		{
			if (sendArgs == null)
			{
				sendArgs = SocketAsyncEventArgsPool.Instance.Take(BufferSize);
				sendArgs.Completed += SendAsyncCompleted;
				sendBuffer = new WriteBuffer(sendArgs.Buffer, sendArgs.Offset, sendArgs.Count);

				recvArgs = SocketAsyncEventArgsPool.Instance.Take(BufferSize);
				recvArgs.Completed += RecvAsyncCompleted;
				recvBuffer = new ReadBuffer(recvArgs.Buffer, recvArgs.Offset, recvArgs.Count);
			}
			else
			{
				sendBuffer.Reset();
				recvBuffer.Reset(0);
			}
		}

		private void RecreateSocket()
		{
			if (LogTraceEnabled) log.Trace("RecreateSocket");
			DestroySocket();

			var tmp = ToTimeout(ReceiveTimeout);
			socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
			{
				NoDelay = true,
				ReceiveBufferSize = BufferSize,
				SendBufferSize = BufferSize,
				ReceiveTimeout = tmp,
				SendTimeout = tmp
			};
		}

		#endregion

		public bool IsAlive
		{
			get { return state == 1; }
			private set { Interlocked.Exchange(ref state, value ? 1 : 0); }
		}

		public ReadBuffer ReadBuffer { get { return recvBuffer; } }
		public WriteBuffer WriteBuffer { get { return sendBuffer; } }

		public int BufferSize
		{
			get { return bufferSize; }
			set
			{
				Require.That(!IsAlive, "Cannot change BufferSize while connected.");
				Require.Value("value", value > 0, "BufferSize must be > 0");

				bufferSize = value;
			}
		}

		public TimeSpan ConnectionTimeout
		{
			get { return connectionTimeout; }
			set
			{
				Require.That(!IsAlive, "Cannot change ConnectionTimeout while connected.");
				Require.Value("value", value >= TimeSpan.Zero, "ConnectionTimeout must be > 0");

				connectionTimeout = value;
			}
		}

		public TimeSpan ReceiveTimeout
		{
			get { return receiveTimeout; }
			set
			{
				Require.That(!IsAlive, "Cannot change ReceiveTimeout while connected.");
				Require.Value("value", value >= TimeSpan.Zero, "ReceiveTimeout must be > 0");

				receiveTimeout = value;
			}
		}

		public void Connect(IPEndPoint endpoint, CancellationToken token)
		{
			InitBuffers();

			this.endpoint = endpoint;
			this.IsAlive = false;

			using (var mre = new ManualResetEventSlim(false))
			using (var opt = new SocketAsyncEventArgs { RemoteEndPoint = endpoint })
			{
				opt.Completed += (a, b) => mre.Set();
				RecreateSocket();

				if (socket.ConnectAsync(opt)
					&& !mre.Wait((int)ConnectionTimeout.TotalMilliseconds, token))
				{
					if (LogTraceEnabled) log.Trace("mre.Wait() timeout while connecting");

					Socket.CancelConnectAsync(opt);
					throw new TimeoutException();
				}

				if (opt.SocketError != SocketError.Success)
					throw new IOException("Could not connect to " + endpoint);

				if (LogDebugEnabled) log.Debug(endpoint + " is connected");

				IsAlive = true;
			}
		}

		public void ScheduleReceive(Action<bool> whenDone)
		{
			if (!IsAlive) whenDone(false);
			else
			{
				recvArgs.UserToken = whenDone;
				if (!socket.ReceiveAsync(recvArgs))
					RecvAsyncCompleted(null, recvArgs);
			}
		}

		void RecvAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			var callback = (Action<bool>)e.UserToken;
			var success = e.SocketError == SocketError.Success && e.BytesTransferred > 0;

			recvBuffer.Reset(success ? e.BytesTransferred : 0);

			callback(success);
		}

		public void ScheduleSend(Action<bool> whenDone)
		{
			if (!IsAlive) whenDone(false);
			else
			{
				sendArgs.UserToken = whenDone;
				PerformSend(sendBuffer.BufferOffset, sendBuffer.Position);
			}
		}

		private void PerformSend(int sendOffset, int sendCount)
		{
			while (true)
			{
				sendArgs.SetBuffer(sendOffset, sendCount);
				if (socket.SendAsync(sendArgs)) break; // will finish asynchronously; event will be triggered

				var sent = sendArgs.BytesTransferred;

				if (sendArgs.SocketError != SocketError.Success || sent < 1)
				{
					// socket error
					((Action<bool>)sendArgs.UserToken)(false);
					break;
				}

				sendOffset += sent;
				sendCount -= sent;

				Debug.Assert(sendCount >= 0);

				if (sendCount == 0)
				{
					// all sent
					sendBuffer.Reset();
					((Action<bool>)sendArgs.UserToken)(true);
					break;
				}
			}
		}

		void SendAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			var sent = sendArgs.BytesTransferred;

			if (sendArgs.SocketError != SocketError.Success || sent < 1)
			{
				((Action<bool>)sendArgs.UserToken)(false);
				return;
			}

			var sendOffset = sendArgs.Offset + sent;
			var sendCount = sendArgs.Count - sent;

			Debug.Assert(sendCount >= 0);

			if (sendCount > 0)
			{
				PerformSend(sendOffset, sendCount);
			}
			else
			{
				sendBuffer.Reset();
				((Action<bool>)sendArgs.UserToken)(true);
			}
		}

		private static int ToTimeout(TimeSpan time)
		{
			return time == TimeSpan.MaxValue
						? Timeout.Infinite
						: (int)time.TotalMilliseconds;
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
