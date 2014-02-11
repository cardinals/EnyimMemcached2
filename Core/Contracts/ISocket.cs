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
