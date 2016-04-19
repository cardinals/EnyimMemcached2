using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using X = System.Linq.Expressions.Expression;

namespace Enyim.Caching.Memcached.Configuration
{
	public static class ObjectUpdater
	{
		private static readonly Dictionary<Type, Updater> Cache = new Dictionary<Type, Updater>();
		private static readonly object CacheLock = new Object();

		public static void Update(object instance, IDictionary<string, string> source)
		{
			if (instance == null) throw new ArgumentNullException(nameof(instance));
			if (source == null) throw new ArgumentNullException(nameof(source));
			if (source.Count == 0) return;

			GetUpdater(instance.GetType()).Update(instance, source);
		}

		#region [ Updater                      ]

		class Updater
		{
			private readonly Dictionary<string, Action<object, string>> cache = new Dictionary<string, Action<object, string>>();

			public Updater(Type type)
			{
				var props = from p in type.GetProperties()
							let bas = p.GetCustomAttributes(typeof(BrowsableAttribute), true)
							let b = bas.FirstOrDefault() as BrowsableAttribute
							where p.CanWrite && (b == null || b.Browsable)
							select p;

				var convert = typeof(Updater).GetMethod("FromString", BindingFlags.Static | BindingFlags.NonPublic);

				foreach (var p in props)
				{
					var instance = X.Parameter(typeof(object));
					var value = X.Parameter(typeof(string));

					var lambda = X.Lambda<Action<object, string>>(
										X.Assign(
											X.Property(
												X.Convert(instance, type),
												p
											),
											X.Convert(
													X.Call(convert, value, X.Constant(p.PropertyType)),
													p.PropertyType
											)
										),
									instance, value);

					cache[p.Name] = lambda.Compile();
				}
			}

			public void Update(object instance, IDictionary<string, string> source)
			{
				foreach (var kvp in source)
				{
					Action<object, string> setter;

					var name = kvp.Key;
					if (String.IsNullOrEmpty(name)) throw new InvalidOperationException("Property name cannot be empty");

					if (!cache.TryGetValue(name, out setter)
						&& !cache.TryGetValue(name.Substring(0, 1).ToUpperInvariant() + name.Substring(1), out setter))
						throw new MissingMemberException(instance.GetType().FullName, kvp.Key);

					setter(instance, kvp.Value);
				}
			}

			private static object FromString(string value, Type targetType)
			{
				return targetType.IsPrimitive
						? Convert.ChangeType(value, targetType)
						: TypeDescriptor.GetConverter(targetType).ConvertFrom(value);
			}
		}

		private static Updater GetUpdater(Type type)
		{
			lock (CacheLock)
			{
				Updater retval;

				if (!Cache.TryGetValue(type, out retval))
				{
					Cache[type] = retval = new Updater(type);
				}

				return retval;
			}
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
