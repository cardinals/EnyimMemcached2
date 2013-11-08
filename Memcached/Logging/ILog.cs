using System;

namespace Enyim.Caching
{
	public interface ILog
	{
		bool IsTraceEnabled { get; }
		bool IsDebugEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsErrorEnabled { get; }
		bool IsFatalEnabled { get; }

		void Trace(object message);
		void Trace(string message, Exception exception);
		void Trace(string format, params object[] args);
		void Trace(IFormatProvider provider, string format, params object[] args);

		void Debug(object message);
		void Debug(string message, Exception exception);
		void Debug(string format, params object[] args);
		void Debug(IFormatProvider provider, string format, params object[] args);

		void Info(object message);
		void Info(string message, Exception exception);
		void Info(string format, params object[] args);
		void Info(IFormatProvider provider, string format, params object[] args);

		void Warn(object message);
		void Warn(string message, Exception exception);
		void Warn(string format, params object[] args);
		void Warn(IFormatProvider provider, string format, params object[] args);

		void Error(object message);
		void Error(string message, Exception exception);
		void Error(string format, params object[] args);
		void Error(IFormatProvider provider, string format, params object[] args);

		void Fatal(object message);
		void Fatal(string message, Exception exception);
		void Fatal(string format, params object[] args);
		void Fatal(IFormatProvider provider, string format, params object[] args);
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
