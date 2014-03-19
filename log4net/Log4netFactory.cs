using System;

namespace Enyim.Caching
{
	public class Log4netFactory : ILogFactory
	{
		public ILog GetLogger(string name)
		{
			return new Ω(log4net.LogManager.GetLogger(name));
		}

		public ILog GetLogger(Type type)
		{
			return new Ω(log4net.LogManager.GetLogger(type.FullName));
		}

		private class Ω : ILog
		{
			private readonly log4net.ILog logger;

			public Ω(log4net.ILog logger)
			{
				this.logger = logger;
			}

			public bool IsTraceEnabled
			{
				get { return logger.IsDebugEnabled; }
			}

			public bool IsDebugEnabled
			{
				get { return logger.IsDebugEnabled; }
			}

			public bool IsInfoEnabled
			{
				get { return logger.IsInfoEnabled; }
			}

			public bool IsWarnEnabled
			{
				get { return logger.IsWarnEnabled; }
			}

			public bool IsErrorEnabled
			{
				get { return logger.IsErrorEnabled; }
			}

			public bool IsFatalEnabled
			{
				get { return logger.IsFatalEnabled; }
			}

			public void Trace(object message)
			{
				// send all Trace message to Debug
				logger.Debug(message);
			}

			public void Trace(string message, Exception exception)
			{
				// send all Trace message to Debug
				logger.Debug(message, exception);
			}

			public void Trace(string format, params object[] args)
			{
				// send all Trace message to Debug
				logger.DebugFormat(format, args);
			}

			public void Trace(IFormatProvider provider, string format, params object[] args)
			{
				// send all Trace message to Debug
				logger.DebugFormat(provider, format, args);
			}

			public void Debug(object message)
			{
				logger.Debug(message);
			}

			public void Debug(string message, Exception exception)
			{
				logger.Debug(message, exception);
			}

			public void Debug(string format, params object[] args)
			{
				logger.DebugFormat(format, args);
			}

			public void Debug(IFormatProvider provider, string format, params object[] args)
			{
				logger.DebugFormat(provider, format, args);
			}

			public void Info(object message)
			{
				logger.Info(message);
			}

			public void Info(string message, Exception exception)
			{
				logger.Info(message, exception);
			}

			public void Info(string format, params object[] args)
			{
				logger.InfoFormat(format, args);
			}

			public void Info(IFormatProvider provider, string format, params object[] args)
			{
				logger.InfoFormat(provider, format, args);
			}

			public void Warn(object message)
			{
				logger.Warn(message);
			}

			public void Warn(string message, Exception exception)
			{
				logger.Warn(message, exception);
			}

			public void Warn(string format, params object[] args)
			{
				logger.WarnFormat(format, args);
			}

			public void Warn(IFormatProvider provider, string format, params object[] args)
			{
				logger.WarnFormat(provider, format, args);
			}

			public void Error(object message)
			{
				logger.Error(message);
			}

			public void Error(string message, Exception exception)
			{
				logger.Error(message, exception);
			}

			public void Error(string format, params object[] args)
			{
				logger.ErrorFormat(format, args);
			}

			public void Error(IFormatProvider provider, string format, params object[] args)
			{
				logger.ErrorFormat(provider, format, args);
			}

			public void Fatal(object message)
			{
				logger.Fatal(message);
			}

			public void Fatal(string message, Exception exception)
			{
				logger.Fatal(message, exception);
			}

			public void Fatal(string format, params object[] args)
			{
				logger.FatalFormat(format, args);
			}

			public void Fatal(IFormatProvider provider, string format, params object[] args)
			{
				logger.FatalFormat(provider, format, args);
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
