using System;
using System.Collections.Immutable;
using System.Threading;

namespace Enyim.Caching.Memcached
{
	internal static class ImmutableInterlocked2
	{
		public static void Remove<T>(ref ImmutableHashSet<T> original, T value)
		{
			var set = Volatile.Read(ref original);

			while (true)
			{
				var newSet = set.Remove(value);
				var tmp = Interlocked.CompareExchange(ref original, newSet, set);

				if (Object.ReferenceEquals(set, tmp)) break;

				set = tmp;
			}
		}

		public static void Add<T>(ref ImmutableHashSet<T> original, T value)
		{
			var set = Volatile.Read(ref original);

			while (true)
			{
				var newSet = set.Add(value);
				var tmp = Interlocked.CompareExchange(ref original, newSet, set);

				if (Object.ReferenceEquals(set, tmp)) break;

				set = tmp;
			}
		}

		public static void Remove<T>(ref ImmutableList<T> original, T value)
		{
			var set = Volatile.Read(ref original);

			while (true)
			{
				var newSet = set.Remove(value);
				var tmp = Interlocked.CompareExchange(ref original, newSet, set);

				if (Object.ReferenceEquals(set, tmp)) break;

				set = tmp;
			}
		}

		public static void Add<T>(ref ImmutableList<T> original, T value)
		{
			var set = Volatile.Read(ref original);

			while (true)
			{
				var newSet = set.Add(value);
				var tmp = Interlocked.CompareExchange(ref original, newSet, set);

				if (Object.ReferenceEquals(set, tmp)) break;

				set = tmp;
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
