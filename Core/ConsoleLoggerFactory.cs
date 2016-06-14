using System;
using System.Collections.Generic;

namespace Enyim.Caching
{
	public class ConsoleLoggerFactory : ILogFactory
	{
		public ILog GetLogger(string name)
		{
			return new Ω(name);
		}

		public ILog GetLogger(Type type)
		{
			return new Ω(type.FullName);
		}

		private class Ω : ILog
		{
			private readonly string name;

			public bool IsTraceEnabled { get { return true; } }
			public bool IsInfoEnabled { get { return true; } }
			public bool IsWarnEnabled { get { return true; } }
			public bool IsErrorEnabled { get { return true; } }

			public Ω(string name)
			{
				this.name = name;
			}

			public void Trace(string message)
			{
				Write(Severities.Trace, message);
			}

			public void Trace(Exception exception, string message = null)
			{
				Write(Severities.Trace, message + " " + exception.ToString());
			}

			public void Trace(string format, params object[] args)
			{
				Write(Severities.Trace, String.Format(format, args));
			}

			public void Info(string message)
			{
				Write(Severities.Info, message);
			}

			public void Info(Exception exception, string message = null)
			{
				Write(Severities.Info, message + " " + exception.ToString());
			}

			public void Info(string format, params object[] args)
			{
				Write(Severities.Info, String.Format(format, args));
			}

			public void Warn(string message)
			{
				Write(Severities.Warn, message);
			}

			public void Warn(Exception exception, string message = null)
			{
				Write(Severities.Warn, message + " " + exception.ToString());
			}

			public void Warn(string format, params object[] args)
			{
				Write(Severities.Warn, String.Format(format, args));
			}

			public void Error(string message)
			{
				Write(Severities.Error, message);
			}

			public void Error(Exception exception, string message = null)
			{
				Write(Severities.Error, message + " " + exception.ToString());
			}

			public void Error(string format, params object[] args)
			{
				Write(Severities.Error, String.Format(format, args));
			}

			private static class Severities
			{
				public const string Trace = "TRACE";
				public const string Info = "INFO ";
				public const string Warn = "WARN ";
				public const string Error = "ERROR";

				public static Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor>
				{
					{ Trace, ConsoleColor.DarkGray },
					{ Info, ConsoleColor.White },
					{ Warn, ConsoleColor.Magenta },
					{ Error, ConsoleColor.Yellow },
				};
			}

			void Write(string severity, string message)
			{
				var c = Console.ForegroundColor;

				Console.ForegroundColor = Severities.Colors[severity];
				Console.WriteLine("{0:HH:mm:ss.fffff} [{1}] {2} - {3}", DateTime.Now, severity, name, message);
				Console.ForegroundColor = c;
			}
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
