using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Enyim.Caching.Tests
{
	public static class MemcachedServer
	{
		static readonly string BasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools");
		static readonly string ExePath = Path.Combine(BasePath, "memcached.exe");

		public static IDisposable Run(int port = 11211)
		{
			var process = Process.Start(new ProcessStartInfo
			{
				Arguments = "-vv -E default_engine.so -p " + port,
				FileName = ExePath,
				WorkingDirectory = BasePath,
				//WindowStyle = ProcessWindowStyle.Hidden
			});

			return new KillProcess(process);
		}

		#region [ KillProcess                  ]

		class KillProcess : IDisposable
		{
			private Process process;

			public KillProcess(Process process)
			{
				this.process = process;
			}

			public void Dispose()
			{
				if (process != null)
				{
					using (process)
						process.Kill();

					process = null;
				}
			}
		}

		#endregion
	}
}
