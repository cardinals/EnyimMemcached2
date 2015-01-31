using System;

namespace Enyim.Caching
{
	public static class Require
	{
		public static void Value(string name, bool valid, string message)
		{
			if (!valid)
				throw new ArgumentOutOfRangeException(name, message);
		}

		public static void That(bool condition, string message)
		{
			if (!condition)
				throw new InvalidOperationException(message);
		}

		public static void NotNull(object value, string parameter, string message = null)
		{
			if (value == null)
				throw new ArgumentNullException(parameter, message);
		}

		public static void Null(object value, string message = null)
		{
			if (value != null)
				throw new ArgumentException(message);
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
