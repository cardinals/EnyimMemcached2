using System;

namespace Enyim.Caching
{
	public class NLogFactory : ILogFactory
	{
		public static void Use()
		{
			LogManager.AssignFactory(new NLogFactory());
		}

		public ILog GetLogger(string name)
		{
			return new Ω(NLog.LogManager.GetLogger(name));
		}

		public ILog GetLogger(Type type)
		{
			return new Ω(NLog.LogManager.GetLogger(type.FullName));
		}

		private class Ω : ILog
		{
			private readonly NLog.Logger logger;

			public Ω(NLog.Logger logger)
			{
				this.logger = logger;
			}

			public bool IsTraceEnabled
			{
				get { return logger.IsTraceEnabled; }
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

			public void Trace(string message)
			{
				logger.Trace(message);
			}

			public void Trace(Exception exception)
			{
				logger.Trace(exception);
			}

			public void Trace(Exception exception, string message = null)
			{
				logger.Trace(exception, message);
			}

			public void Trace(string format, params object[] args)
			{
				logger.Trace(format, args);
			}

			public void Debug(string message)
			{
				logger.Debug(message);
			}

			public void Debug(Exception exception, string message = null)
			{
				logger.Debug(exception, message);
			}

			public void Debug(string format, params object[] args)
			{
				logger.Debug(format, args);
			}

			public void Info(string message)
			{
				logger.Info(message);
			}

			public void Info(Exception exception, string message = null)
			{
				logger.Info(exception, message);
			}

			public void Info(string format, params object[] args)
			{
				logger.Info(format, args);
			}

			public void Warn(string message)
			{
				logger.Warn(message);
			}

			public void Warn(Exception exception, string message = null)
			{
				logger.Warn(exception, message);
			}

			public void Warn(string format, params object[] args)
			{
				logger.Warn(format, args);
			}

			public void Error(string message)
			{
				logger.Error(message);
			}

			public void Error(Exception exception, string message = null)
			{
				logger.Error(exception, message);
			}

			public void Error(string format, params object[] args)
			{
				logger.Error(format, args);
			}

			public void Fatal(string message)
			{
				logger.Fatal(message);
			}

			public void Fatal(Exception exception, string message = null)
			{
				logger.Fatal(exception, message);
			}

			public void Fatal(string format, params object[] args)
			{
				logger.Fatal(format, args);
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
