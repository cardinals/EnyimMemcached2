using System;
using System.Runtime.CompilerServices;

namespace Enyim.Caching
{
	public static class LogManager
	{
		private static ILogFactory factory = new NullLoggerFactory();

		public static void AssignFactory(ILogFactory factory)
		{
			LogManager.factory = factory;
		}

		public static ILog GetLogger(string name)
		{
			return factory.GetLogger(name);
		}

		public static ILog GetLogger(Type type)
		{
			return factory.GetLogger(type);
		}

		#region [ NullLoggerFactory            ]

		private class NullLoggerFactory : ILogFactory
		{
			private static readonly ILog Instance = new Ω();

			public ILog GetLogger(string name)
			{
				return Instance;
			}

			public ILog GetLogger(Type type)
			{
				return Instance;
			}

			#region [ Logger                       ]

			private class Ω : ILog
			{
				public bool IsTraceEnabled { get { return false; } }
				public bool IsDebugEnabled { get { return false; } }
				public bool IsInfoEnabled { get { return false; } }
				public bool IsWarnEnabled { get { return false; } }
				public bool IsErrorEnabled { get { return false; } }
				public bool IsFatalEnabled { get { return false; } }

				public void Trace(string message) { }
				public void Trace(Exception exception) { }
				public void Trace(Exception exception, string message) { }
				public void Trace(string format, params object[] args) { }
				public void Trace(IFormatProvider provider, string format, params object[] args) { }

				public void Debug(string message) { }
				public void Debug(Exception exception) { }
				public void Debug(Exception exception, string message) { }
				public void Debug(string format, params object[] args) { }
				public void Debug(IFormatProvider provider, string format, params object[] args) { }

				public void Info(string message) { }
				public void Info(Exception exception) { }
				public void Info(Exception exception, string message) { }
				public void Info(string format, params object[] args) { }
				public void Info(IFormatProvider provider, string format, params object[] args) { }

				public void Warn(string message) { }
				public void Warn(Exception exception) { }
				public void Warn(Exception exception, string message) { }
				public void Warn(string format, params object[] args) { }
				public void Warn(IFormatProvider provider, string format, params object[] args) { }

				public void Error(string message) { }
				public void Error(Exception exception) { }
				public void Error(Exception exception, string message) { }
				public void Error(string format, params object[] args) { }
				public void Error(IFormatProvider provider, string format, params object[] args) { }

				public void Fatal(string message) { }
				public void Fatal(Exception exception) { }
				public void Fatal(Exception exception, string message) { }
				public void Fatal(string format, params object[] args) { }
				public void Fatal(IFormatProvider provider, string format, params object[] args) { }
			}

			#endregion
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
