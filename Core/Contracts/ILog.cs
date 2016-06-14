using System;

namespace Enyim.Caching
{
	public interface ILog
	{
		bool IsTraceEnabled { get; }
		bool IsInfoEnabled { get; }
		bool IsWarnEnabled { get; }
		bool IsErrorEnabled { get; }

		void Trace(string message);
		void Trace(Exception exception, string message = null);
		void Trace(string format, params object[] args);

		void Info(string message);
		void Info(Exception exception, string message = null);
		void Info(string format, params object[] args);

		void Warn(string message);
		void Warn(Exception exception, string message = null);
		void Warn(string format, params object[] args);

		void Error(string message);
		void Error(Exception exception, string message = null);
		void Error(string format, params object[] args);
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
