using System;
using Serilog;
using Serilog.Events;

namespace Enyim.Caching
{
	public class SerilogFactory : ILogFactory
	{
		private readonly LoggerConfiguration config;

		private SerilogFactory(LoggerConfiguration config)
		{
			this.config = config;
		}

		public static void Use(LoggerConfiguration config)
		{
			LogManager.AssignFactory(new SerilogFactory(config));
		}

		public ILog GetLogger(string name)
		{
			return new Ω(config.CreateLogger().ForContext("Name", name));
		}

		public ILog GetLogger(Type type)
		{
			return new Ω(config.CreateLogger().ForContext(type));
		}

		private class Ω : ILog
		{
			private readonly ILogger logger;

			public Ω(ILogger logger)
			{
				this.logger = logger;
			}

			public bool IsTraceEnabled { get { return logger.IsEnabled(LogEventLevel.Verbose); } }
			public bool IsInfoEnabled { get { return logger.IsEnabled(LogEventLevel.Information); } }
			public bool IsWarnEnabled { get { return logger.IsEnabled(LogEventLevel.Warning); } }
			public bool IsErrorEnabled { get { return logger.IsEnabled(LogEventLevel.Error); } }

			public void Trace(string message)
			{
				logger.Verbose(message);
			}

			public void Trace(Exception exception, string message)
			{
				logger.Verbose(exception, message);
			}

			public void Trace(string format, params object[] args)
			{
				logger.Verbose(format, args);
			}

			public void Info(string message)
			{
				logger.Information(message);
			}

			public void Info(Exception exception, string message = null)
			{
				logger.Information(exception, message);
			}

			public void Info(string format, params object[] args)
			{
				logger.Information(format, args);
			}

			public void Warn(string message)
			{
				logger.Warning(message);
			}

			public void Warn(Exception exception, string message = null)
			{
				logger.Warning(exception, message);
			}

			public void Warn(string format, params object[] args)
			{
				logger.Warning(format, args);
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
