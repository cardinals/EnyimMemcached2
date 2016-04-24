using System;
using System.Linq;
using System.Collections.Generic;

namespace Enyim.Caching
{
	/// <summary>
	/// Helper for writing logging code. Calls to this calls will be rewritten to guarded ILog invokations by the LogTo weaver.
	/// </summary>
	public static class LogTo
	{
		public static void Trace(string message) { }
		public static void Trace(Exception exception, string message = null) { }
		public static void Trace(string format, params object[] args) { }

		public static void Debug(string message) { }
		public static void Debug(Exception exception, string message = null) { }
		public static void Debug(string format, params object[] args) { }

		public static void Info(string message) { }
		public static void Info(Exception exception, string message = null) { }
		public static void Info(string format, params object[] args) { }

		public static void Warn(string message) { }
		public static void Warn(Exception exception, string message = null) { }
		public static void Warn(string format, params object[] args) { }

		public static void Error(string message) { }
		public static void Error(Exception e, string message = null) { }
		public static void Error(string format, params object[] args) { }

		public static void Fatal(string message) { }
		public static void Fatal(Exception exception, string message = null) { }
		public static void Fatal(string format, params object[] args) { }
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
