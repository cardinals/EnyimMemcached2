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
				Arguments =
#if DEBUG
				"-vv " +
#endif
				$"-E default_engine.so -p {port} -m 512",
				FileName = ExePath,
				WorkingDirectory = BasePath
#if !DEBUG
				,WindowStyle = ProcessWindowStyle.Hidden
#endif
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
