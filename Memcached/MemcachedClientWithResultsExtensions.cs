using System;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public static class MemcachedClientWithResultsExtensions
	{
		public static IGetOperationResult<object> Get(this IMemcachedClientWithResults self, string key)
		{
			return self.Get<object>(key);
		}

		public static Task<IGetOperationResult<object>> GetAsync(this IMemcachedClientWithResults self, string key)
		{
			return self.GetAsync<object>(key);
		}

		public static IOperationResult Append(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = 0)
		{
			return self.Append(key, new ArraySegment<byte>(data), cas);
		}

		public static IOperationResult Append(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.Concate(ConcatenationMode.Append, key, data, cas);
		}

		public static IOperationResult Prepend(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = 0)
		{
			return self.Prepend(key, new ArraySegment<byte>(data), cas);
		}

		public static IOperationResult Prepend(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.Concate(ConcatenationMode.Prepend, key, data, cas);
		}

		public static IMutateOperationResult Increment(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.Mutate(MutationMode.Increment, key, defaultValue, delta, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static IMutateOperationResult Decrement(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.Mutate(MutationMode.Decrement, key, defaultValue, delta, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static Task<IOperationResult> AppendAsync(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = 0)
		{
			return self.AppendAsync(key, new ArraySegment<byte>(data), cas);
		}

		public static Task<IOperationResult> AppendAsync(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.ConcateAsync(ConcatenationMode.Append, key, data, cas);
		}

		public static Task<IOperationResult> PrependAsync(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = 0)
		{
			return self.PrependAsync(key, new ArraySegment<byte>(data), cas);
		}

		public static Task<IOperationResult> PrependAsync(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.ConcateAsync(ConcatenationMode.Prepend, key, data, cas);
		}

		public static Task<IMutateOperationResult> IncrementAsync(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.MutateAsync(MutationMode.Increment, key, defaultValue, delta, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static Task<IMutateOperationResult> DecrementAsync(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.MutateAsync(MutationMode.Decrement, key, defaultValue, delta, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static IOperationResult Add(this IMemcachedClientWithResults self, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.Store(StoreMode.Add, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static Task<IOperationResult> AddAsync(this IMemcachedClientWithResults self, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.StoreAsync(StoreMode.Add, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static IOperationResult Replace(this IMemcachedClientWithResults self, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.Store(StoreMode.Replace, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static Task<IOperationResult> ReplaceAsync(this IMemcachedClientWithResults self, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static IOperationResult Set(this IMemcachedClientWithResults self, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.Store(StoreMode.Set, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static Task<IOperationResult> SetAsync(this IMemcachedClientWithResults self, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.StoreAsync(StoreMode.Set, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static IOperationResult Store(this IMemcachedClientWithResults self, StoreMode mode, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.Store(mode, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static Task<IOperationResult> StoreAsync(this IMemcachedClientWithResults self, StoreMode mode, string key, object value, ulong cas = 0, TimeSpan? validFor = null, DateTime? expiresAt = null)
		{
			return self.StoreAsync(mode, key, value, cas, MemcachedClientExtensions.MakeExpiration(validFor, expiresAt));
		}

		public static IOperationResult Remove(this IMemcachedClientWithResults self, string key)
		{
			return self.Remove(key, 0);
		}

		public static Task<IOperationResult> RemoveAsync(this IMemcachedClientWithResults self, string key)
		{
			return self.RemoveAsync(key, 0);
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
