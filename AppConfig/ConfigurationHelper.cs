using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Enyim.Caching.Configuration
{
	internal static class ConfigurationHelper
	{
		public static bool TryGetAndRemove(IDictionary<string, string> dict, string name, out int value, bool required)
		{
			string tmp;
			if (TryGetAndRemove(dict, name, out tmp, required)
				&& Int32.TryParse(tmp, out value))
			{
				return true;
			}

			if (required)
				throw new System.Configuration.ConfigurationErrorsException("Missing or invalid parameter: " + (String.IsNullOrEmpty(name) ? "element content" : name));

			value = 0;

			return false;
		}

		public static TimeSpan GetAndRemove(IDictionary<string, string> dict, string name, bool required, TimeSpan @default)
		{
			TimeSpan tmp;

			return TryGetAndRemove(dict, name, out tmp, required) ? tmp : @default;
		}

		public static int GetAndRemove(IDictionary<string, string> dict, string name, bool required, int @default)
		{
			int tmp;

			return TryGetAndRemove(dict, name, out tmp, required) ? tmp : @default;
		}

		public static bool TryGetAndRemove(IDictionary<string, string> dict, string name, out TimeSpan value, bool required)
		{
			string tmp;
			if (TryGetAndRemove(dict, name, out tmp, required)
				&& TimeSpan.TryParse(tmp, out value))
			{
				return true;
			}

			if (required)
				throw new System.Configuration.ConfigurationErrorsException("Missing or invalid parameter: " + (String.IsNullOrEmpty(name) ? "element content" : name));

			value = TimeSpan.Zero;

			return false;
		}

		public static bool TryGetAndRemove(IDictionary<string, string> dict, string name, out string value, bool required)
		{
			if (dict.TryGetValue(name, out value))
			{
				dict.Remove(name);

				if (!String.IsNullOrEmpty(value))
					return true;
			}

			if (required)
				throw new System.Configuration.ConfigurationErrorsException("Missing parameter: " + (String.IsNullOrEmpty(name) ? "element content" : name));

			return false;
		}

		public static void CheckForUnknownAttributes(IDictionary<string, string> dict)
		{
			if (dict.Count > 0)
				throw new System.Configuration.ConfigurationErrorsException("Unrecognized parameter: " + dict.Keys.First());
		}

		public static void CheckForInterface(Type type, Type interfaceType)
		{
			if (type == null || interfaceType == null) return;

			if (Array.IndexOf<Type>(type.GetInterfaces(), interfaceType) == -1)
				throw new System.Configuration.ConfigurationErrorsException("The type " + type.AssemblyQualifiedName + " must implement " + interfaceType.AssemblyQualifiedName);
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
