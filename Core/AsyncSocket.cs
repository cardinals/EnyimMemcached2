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
		public static class Defaults
		{
			public const int MaxBufferSize = 1 * 1024 * 1024;
			public const int MinBufferSize = 4096;

			public const int SendBufferSize = 32 * 1024;
			public const int ReceiveBufferSize = 32 * 1024;

			public const int ConnectionTimeoutMsec = 10000;
			public const int SendTimeoutMsec = 10000;
			public const int ReceiveTimeoutMsec = 10000;
		}

		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private static readonly bool LogTraceEnabled = log.IsTraceEnabled;
		private static readonly bool LogDebugEnabled = log.IsDebugEnabled;

		private readonly object ConnectLock = new object();

		private IPEndPoint endpoint;
		private TimeSpan connectionTimeout;
		private TimeSpan sendTimeout;
		private TimeSpan receiveTimeout;
		private int sendBufferSize;
		private int receiveBufferSize;

		private Socket socket;
		private int isAlive;

		private SocketAsyncEventArgs recvArgs;
		private ReadBuffer recvBuffer;

		private SocketAsyncEventArgs sendArgs;
		private WriteBuffer sendBuffer;

		private ManualResetEventSlim notWorking = new ManualResetEventSlim();

		public AsyncSocket()
		{
			ConnectionTimeout = TimeSpan.FromMilliseconds(Defaults.ConnectionTimeoutMsec);
			SendTimeout = TimeSpan.FromMilliseconds(Defaults.SendTimeoutMsec);
			ReceiveTimeout = TimeSpan.FromMilliseconds(Defaults.ReceiveTimeoutMsec);

			SendBufferSize = Defaults.SendBufferSize;
			ReceiveBufferSize = Defaults.ReceiveBufferSize;
		}

		public void WaitOne(CancellationToken token)
		{
			notWorking.Wait(token);
		}

		//private int socketState;

		//public SocketState State
		//{
		//	get { return (SocketState)Volatile.Read(ref socketState); }
		//	private set { Interlocked.Exchange(ref socketState, (int)value); }
		//}

		public void Connect(IPEndPoint endpoint, CancellationToken token)
		{
			lock (ConnectLock)
			{
				PerformConnect(endpoint, token);
			}
		}

		private void PerformConnect(IPEndPoint endpoint, CancellationToken token)
		{
			InitBuffers();

			this.endpoint = endpoint;
			this.IsAlive = false;

			using (var mre = new ManualResetEventSlim(false))
			using (var opt = new SocketAsyncEventArgs { RemoteEndPoint = endpoint })
			{
				opt.Completed += (a, b) => mre.Set();
				RecreateSocket();

				//State = SocketState.Connecting;
				notWorking.Reset();

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
				notWorking.Set();

				//State = SocketState.Free;
			}
		}

		public void ScheduleSend(Action<bool> whenDone)
		{
			notWorking.Reset();
			//State = SocketState.Sending;

			if (!IsAlive)
			{
				whenDone(false);
				notWorking.Set();

				//State = SocketState.Free;
			}
			else
			{
				sendArgs.UserToken = whenDone;
				PerformSend(sendBuffer.BufferOffset, sendBuffer.Position);
			}
		}

		private void PerformSend(int sendOffset, int sendCount)
		{
		loop:

			// try sending all our data
			sendArgs.SetBuffer(sendOffset, sendCount);
			// send is done asynchrously (SendAsyncCompleted will finish up)
			if (socket.SendAsync(sendArgs)) return;

			// send was done synchronously
			var sent = sendArgs.BytesTransferred;

			// check for fail
			if (sendArgs.SocketError != SocketError.Success || sent < 1)
			{
				// socket error
				FinishSending(false);
				return;
			}

			sendOffset += sent;
			sendCount -= sent;

			Debug.Assert(sendCount >= 0);

			// no data is remaining: quit
			// otherwise try sending a new chunk
			if (sendCount == 0)
			{
				sendBuffer.Reset();
				FinishSending(true);
				return;
			}

			goto loop;
		}

		private void SendAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			var sent = sendArgs.BytesTransferred;

			// failed during send
			if (sendArgs.SocketError != SocketError.Success || sent < 1)
			{
				FinishSending(false);
				return;
			}

			var sendOffset = sendArgs.Offset + sent;
			var sendCount = sendArgs.Count - sent;

			Debug.Assert(sendCount >= 0);

			// OS sent less data than we asked for,
			// send the remaining data
			if (sendCount > 0)
			{
				PerformSend(sendOffset, sendCount);
			}
			else
			{
				// all sent successfully
				sendBuffer.Reset();
				FinishSending(true);
			}
		}

		private void FinishSending(bool success)
		{
			((Action<bool>)sendArgs.UserToken)(success);
			notWorking.Set();
			//State = SocketState.Free;
		}

		private void FinishReceiving(bool success)
		{
			((Action<bool>)recvArgs.UserToken)(success);
			notWorking.Set();
			//State = SocketState.Free;
		}

		public void ScheduleReceive(Action<bool> whenDone)
		{
			notWorking.Reset();

			//State = SocketState.Receiving;

			if (!IsAlive)
			{
				whenDone(false);
				notWorking.Set();

				//State = SocketState.Free;
			}
			else
			{
				recvArgs.UserToken = whenDone;

				if (LogTraceEnabled) log.Trace("Socket {0} is receiving", endpoint);

				if (!socket.ReceiveAsync(recvArgs))
				{
					if (LogTraceEnabled) log.Trace("Socket {0} received synchronously", endpoint);
					RecvAsyncCompleted(null, recvArgs);
				}
			}
		}

		private void RecvAsyncCompleted(object sender, SocketAsyncEventArgs e)
		{
			var success = e.SocketError == SocketError.Success && e.BytesTransferred > 0;
			if (LogTraceEnabled) log.Trace("Socket {0} success: {1}, bytes {2}", endpoint, success, e.BytesTransferred);

			recvBuffer.SetAvailableLength(success ? e.BytesTransferred : 0);

			FinishReceiving(success);
		}

		private static int ToTimeout(TimeSpan time)
		{
			return time == TimeSpan.MaxValue
						? Timeout.Infinite
						: (int)time.TotalMilliseconds;
		}

		#region [ Property noise               ]

		public bool IsAlive
		{
			get { return isAlive == 1; }
			private set { Interlocked.Exchange(ref isAlive, value ? 1 : 0); }
		}

		public ReadBuffer ReadBuffer { get { return recvBuffer; } }
		public WriteBuffer WriteBuffer { get { return sendBuffer; } }

		private void ThrowIfConnected()
		{
			Require.That(!IsAlive, "Cannot change socket options while connected.");
		}

		public TimeSpan ConnectionTimeout
		{
			get { return connectionTimeout; }
			set
			{
				ThrowIfConnected();
				Require.Value("value", value >= TimeSpan.Zero, "ConnectionTimeout must be > 0");

				connectionTimeout = value;
			}
		}

		public TimeSpan SendTimeout
		{
			get { return sendTimeout; }
			set
			{
				ThrowIfConnected();
				Require.Value("value", value >= TimeSpan.Zero, "SendTimeout must be > 0");

				sendTimeout = value;
			}
		}

		public TimeSpan ReceiveTimeout
		{
			get { return receiveTimeout; }
			set
			{
				ThrowIfConnected();
				Require.Value("value", value >= TimeSpan.Zero, "ReceiveTimeout must be > 0");

				receiveTimeout = value;
			}
		}

		public int SendBufferSize
		{
			get { return sendBufferSize; }
			set
			{
				ThrowIfConnected();
				Require.Value("value", value >= Defaults.MinBufferSize, "SendBufferSize must be >= " + Defaults.MinBufferSize);
				Require.Value("value", value <= Defaults.MaxBufferSize, "SendBufferSize must be <=- " + Defaults.MaxBufferSize);
				Require.Value("value", value % 4096 == 0, "SendBufferSize must be a multiply of 4k");

				sendBufferSize = value;
			}
		}

		public int ReceiveBufferSize
		{
			get { return receiveBufferSize; }
			set
			{
				ThrowIfConnected();
				Require.Value("value", value >= Defaults.MinBufferSize, "ReceiveBufferSize must be > 0" + Defaults.MinBufferSize);
				Require.Value("value", value <= Defaults.MaxBufferSize, "ReceiveBufferSize must be <=- " + Defaults.MaxBufferSize);
				Require.Value("value", value % 4096 == 0, "ReceiveBufferSize must be a multiply of 4k");

				receiveBufferSize = value;
			}
		}

		#endregion
		#region [ Cleanup                      ]

		~AsyncSocket()
		{
			try { this.Dispose(); }
			catch { }
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			lock (ConnectLock)
			{
				sendArgs.Completed -= SendAsyncCompleted;
				recvArgs.Completed -= RecvAsyncCompleted;

				SocketAsyncEventArgsFactory.Instance.Return(recvArgs);
				SocketAsyncEventArgsFactory.Instance.Return(sendArgs);

				DestroySocket();
			}
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
				sendArgs = SocketAsyncEventArgsFactory.Instance.Take(SendBufferSize);
				sendArgs.Completed += SendAsyncCompleted;
				sendBuffer = new WriteBuffer(sendArgs.Buffer, sendArgs.Offset, sendArgs.Count);

				recvArgs = SocketAsyncEventArgsFactory.Instance.Take(ReceiveBufferSize);
				recvArgs.Completed += RecvAsyncCompleted;
				recvBuffer = new ReadBuffer(recvArgs.Buffer, recvArgs.Offset, recvArgs.Count);
			}
			else
			{
				sendBuffer.Reset();
				recvBuffer.SetAvailableLength(0);
			}
		}

		private void RecreateSocket()
		{
			if (LogTraceEnabled) log.Trace("RecreateSocket; destroying previous");
			DestroySocket();

			socket = new Socket(SocketType.Stream, ProtocolType.Tcp)
			{
				NoDelay = true,
				ReceiveBufferSize = ReceiveBufferSize,
				SendBufferSize = SendBufferSize,
				ReceiveTimeout = ToTimeout(ReceiveTimeout),
				SendTimeout = ToTimeout(SendTimeout)
			};

			if (LogTraceEnabled) log.Trace("Socket was recreated");
		}

		#endregion
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
