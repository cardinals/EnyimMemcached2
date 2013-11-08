using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Enyim.Caching.Memcached
{
	[DebuggerDisplay("[ Address: {endpoint}, IsAlive = {IsAlive} ]")]
	public class SafeSocket : ISocket
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private readonly object InstanceLock = new object();

		private IPEndPoint endpoint;
		private int bufferSize;
		private TimeSpan receiveTimeout;
		private TimeSpan connectionTimeout;

		private Socket socket;
		private int state;

		public SafeSocket()
		{
			receiveTimeout = TimeSpan.FromSeconds(10);
			ConnectionTimeout = TimeSpan.FromSeconds(10);
			BufferSize = 16 * 1024;
		}

		~SafeSocket()
		{
			try { this.Dispose(); }
			catch { }
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			DestroySocket();
		}

		public void Connect(IPEndPoint endpoint, CancellationToken token)
		{
			lock (InstanceLock)
				PerformConnect(endpoint, token);
		}

		private void PerformConnect(IPEndPoint endpoint, CancellationToken token)
		{
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
					if (log.IsTraceEnabled) log.Trace("mre.Wait() timeout while connecting");

					Socket.CancelConnectAsync(opt);
					throw new TimeoutException();
				}

				if (opt.SocketError == SocketError.Success)
				{
					if (log.IsDebugEnabled) log.Debug(endpoint + " is connected");
					IsAlive = true;
				}
				else
				{
					throw new IOException("Could not connect to " + endpoint);
				}
			}
		}

		/// <summary>
		/// Reads from the socket.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns>The amount of bytes read.</returns>
		public int Receive(byte[] buffer, int offset, int count)
		{
			if (count < 1) return count;

			SocketError errorCode;
			var read = socket.Receive(buffer, offset, count, SocketFlags.None, out errorCode);

			// read=0 means we must reconnect
			if (errorCode != SocketError.Success || read < 1)
				ThrowIOE("Could not read, reason: " + errorCode);

			return read;
		}

		/// <summary>
		/// Sends the data.
		/// </summary>
		public void Send(WriteBuffer buffer)
		{
			Send(buffer.GetBuffer(), 0, buffer.Position);
			buffer.Reset();
		}

		/// <summary>
		/// Sends the data.
		/// </summary>
		public void Send(byte[] buffer, int offset, int count)
		{
			if (count < 1) return;

			SocketError errorCode;
			var sent = socket.Send(buffer, offset, count, SocketFlags.None, out errorCode);

			// sent=0 means we must reconnect
			if (errorCode != SocketError.Success || sent < 1)
				ThrowIOE("Could not write, reason: " + errorCode);
		}

		public bool IsAlive
		{
			get { return state == 1; }
			private set
			{
				Interlocked.Exchange(ref state, value ? 1 : 0);
				Thread.MemoryBarrier();
			}
		}

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

		private void RecreateSocket()
		{
			if (log.IsTraceEnabled) log.Trace("RecreateSocket");
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

		private void DestroySocket()
		{
			if (log.IsTraceEnabled) log.Trace("DestroySocket");

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
					if (log.IsDebugEnabled) log.Debug("Exception while destroying socket.", e);
				}

				socket = null;
			}
		}

		private static int ToTimeout(TimeSpan time)
		{
			return time == TimeSpan.MaxValue
						? Timeout.Infinite
						: (int)time.TotalMilliseconds;
		}

		private void ThrowIOE(string message)
		{
			throw new IOException(endpoint + " has failed. " + message);
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
