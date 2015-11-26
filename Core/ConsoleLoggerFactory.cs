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
			public bool IsDebugEnabled { get { return true; } }
			public bool IsInfoEnabled { get { return true; } }
			public bool IsWarnEnabled { get { return true; } }
			public bool IsErrorEnabled { get { return true; } }
			public bool IsFatalEnabled { get { return true; } }

			public Ω(string name)
			{
				this.name = name;
			}

			public void Trace(object message)
			{
				Write(Severities.Trace, String.Format("{0}", message));
			}

			public void Trace(string message, Exception exception)
			{
				Write(Severities.Trace, String.Format("{0} {1}", message, exception));
			}

			public void Trace(string format, params object[] args)
			{
				Write(Severities.Trace, String.Format(format, args));
			}

			public void Trace(IFormatProvider provider, string format, params object[] args)
			{
				Write(Severities.Trace, String.Format(provider, format, args));
			}

			public void Debug(object message)
			{
				Write(Severities.Debug, String.Format("{0}", message));
			}

			public void Debug(string message, Exception exception)
			{
				Write(Severities.Debug, String.Format("{0} {1}", message, exception));
			}

			public void Debug(string format, params object[] args)
			{
				Write(Severities.Debug, String.Format(format, args));
			}

			public void Debug(IFormatProvider provider, string format, params object[] args)
			{
				Write(Severities.Debug, String.Format(provider, format, args));
			}

			public void Info(object message)
			{
				Write(Severities.Info, String.Format("{0}", message));
			}

			public void Info(string message, Exception exception)
			{
				Write(Severities.Info, String.Format("{0} {1}", message, exception));
			}

			public void Info(string format, params object[] args)
			{
				Write(Severities.Info, String.Format(format, args));
			}

			public void Info(IFormatProvider provider, string format, params object[] args)
			{
				Write(Severities.Info, String.Format(provider, format, args));
			}

			public void Warn(object message)
			{
				Write(Severities.Warn, String.Format("{0}", message));
			}

			public void Warn(string message, Exception exception)
			{
				Write(Severities.Warn, String.Format("{0} {1}", message, exception));
			}

			public void Warn(string format, params object[] args)
			{
				Write(Severities.Warn, String.Format(format, args));
			}

			public void Warn(IFormatProvider provider, string format, params object[] args)
			{
				Write(Severities.Warn, String.Format(provider, format, args));
			}

			public void Error(object message)
			{
				Write(Severities.Error, String.Format("{0}", message));
			}

			public void Error(string message, Exception exception)
			{
				Write(Severities.Error, String.Format("{0} {1}", message, exception));
			}

			public void Error(string format, params object[] args)
			{
				Write(Severities.Error, String.Format(format, args));
			}

			public void Error(IFormatProvider provider, string format, params object[] args)
			{
				Write(Severities.Error, String.Format(provider, format, args));
			}

			public void Fatal(object message)
			{
				Write(Severities.Fatal, String.Format("{0}", message));
			}

			public void Fatal(string message, Exception exception)
			{
				Write(Severities.Fatal, String.Format("{0} {1}", message, exception));
			}

			public void Fatal(string format, params object[] args)
			{
				Write(Severities.Fatal, String.Format(format, args));
			}

			public void Fatal(IFormatProvider provider, string format, params object[] args)
			{
				Write(Severities.Fatal, String.Format(provider, format, args));
			}

			private static class Severities
			{
				public const string Trace = "TRACE";
				public const string Debug = "DEBUG";
				public const string Info = "INFO ";
				public const string Warn = "WARN ";
				public const string Error = "ERROR";
				public const string Fatal = "FATAL";

				public static Dictionary<string, ConsoleColor> Colors = new Dictionary<string, ConsoleColor>
				{
					{ Trace, ConsoleColor.DarkGray },
					{ Debug, ConsoleColor.Gray },
					{ Info, ConsoleColor.White },
					{ Warn, ConsoleColor.Magenta },
					{ Error, ConsoleColor.Yellow },
					{ Fatal , ConsoleColor.Red }
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
