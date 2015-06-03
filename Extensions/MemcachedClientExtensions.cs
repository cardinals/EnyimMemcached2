using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public static class MemcachedClientNoExpireExtensions
	{
		public static Task<bool> StoreAsync(this IMemcachedClient self, StoreMode mode, string key, object value)
		{
			return self.StoreAsync(mode, key, value, DateTime.MinValue);
		}

		public static Task<T> GetAndTouchAsync<T>(this IMemcachedClient self, string key)
		{
			return self.GetAndTouchAsync<T>(key, DateTime.MinValue);
		}

		public static Task<bool> TouchAsync(this IMemcachedClient self, string key)
		{
			return self.TouchAsync(key, DateTime.MinValue);
		}

		public static Task<ulong> MutateAsync(this IMemcachedClient self, MutationMode mode, string key, ulong defaultValue = 1, ulong delta = 1)
		{
			return self.MutateAsync(mode, key, DateTime.MinValue, defaultValue, delta);
		}
	}

	public static class MemcachedClientWRNoExpireExtensions
	{
		public static Task<IOperationResult> StoreAsync(this IMemcachedClientWithResults self, StoreMode mode, string key, object value, ulong cas = Protocol.NO_CAS)
		{
			return self.StoreAsync(mode, key, value, DateTime.MinValue, cas);
		}

		public static Task<IGetOperationResult<T>> GetAndTouchAsync<T>(this IMemcachedClientWithResults self, string key, ulong cas = Protocol.NO_CAS)
		{
			return self.GetAndTouchAsync<T>(key, DateTime.MinValue);
		}

		public static Task<IOperationResult> TouchAsync(this IMemcachedClientWithResults self, string key, ulong cas = Protocol.NO_CAS)
		{
			return self.TouchAsync(key, DateTime.MinValue, cas);
		}

		public static Task<IMutateOperationResult> MutateAsync(this IMemcachedClientWithResults self, MutationMode mode, string key, ulong defaultValue = Protocol.MUTATE_DEFAULT_VALUE, ulong delta = Protocol.MUTATE_DEFAULT_DELTA, ulong cas = Protocol.NO_CAS)
		{
			return self.MutateAsync(mode, key, DateTime.MinValue, defaultValue, delta, cas);
		}
	}

	public static class MemcachedClientExtensions
	{
		private static T Run<T>(Task<T> task)
		{
			try
			{
				return task.Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		#region Store

		public static Task<bool> StoreAsync(this IMemcachedClient self, StoreMode mode, string key, object value)
		{
			return self.StoreAsync(mode, key, value, DateTime.MinValue);
		}

		public static bool Store(this IMemcachedClient self, StoreMode mode, string key, object value)
		{
			return Run(self.StoreAsync(mode, key, value, DateTime.MinValue));
		}

		public static bool Store(this IMemcachedClient self, StoreMode mode, string key, object value, DateTime expiresAt)
		{
			return Run(self.StoreAsync(mode, key, value, expiresAt));
		}

		public static bool Store(this IMemcachedClient self, StoreMode mode, string key, object value, TimeSpan validFor)
		{
			return Run(self.StoreAsync(mode, key, value, validFor));
		}

		#endregion
		#region GetAndTouch

		public static Task<T> GetAndTouchAsync<T>(this IMemcachedClient self, string key)
		{
			return self.GetAndTouchAsync<T>(key, DateTime.MinValue);
		}

		public static T GetAndTouch<T>(this IMemcachedClient self, string key, DateTime expiration)
		{
			return Run(self.GetAndTouchAsync<T>(key, expiration));
		}

		public static T GetAndTouch<T>(this IMemcachedClient self, string key, TimeSpan validFor)
		{
			return Run(self.GetAndTouchAsync<T>(key, validFor));
		}

		#endregion
		#region Touch

		public static Task<bool> TouchAsync(this IMemcachedClient self, string key)
		{
			return self.TouchAsync(key, DateTime.MinValue);
		}

		public static bool Touch(this IMemcachedClient self, string key, DateTime expiration)
		{
			return Run(self.TouchAsync(key, expiration));
		}

		public static bool Touch(this IMemcachedClient self, string key, TimeSpan validFor)
		{
			return Run(self.TouchAsync(key, validFor));
		}

		#endregion


		#region noexpire

		const ulong MUTATE_DEFAULT_VALUE = 1;
		const ulong MUTATE_DEFAULT_DELTA = 1;

		public static Task<ulong> MutateAsync(this IMemcachedClient self, MutationMode mode, string key, ulong defaultValue = MUTATE_DEFAULT_VALUE, ulong delta = MUTATE_DEFAULT_DELTA)
		{
			return self.MutateAsync(mode, key, DateTime.MinValue, defaultValue, delta);
		}

		public static ulong Mutate(this IMemcachedClient self, MutationMode mode, string key, ulong defaultValue = MUTATE_DEFAULT_VALUE, ulong delta = MUTATE_DEFAULT_DELTA, DateTime? expiresAt = null)
		{
			return Run(self.MutateAsync(mode, key, expiresAt ?? DateTime.MinValue, defaultValue, delta));
		}

		public static ulong Mutate(this IMemcachedClient self, MutationMode mode, string key, ulong defaultValue = MUTATE_DEFAULT_VALUE, ulong delta = MUTATE_DEFAULT_DELTA, TimeSpan? validFor = null)
		{
			return Run(self.MutateAsync(mode, key, validFor ?? TimeSpan.Zero, defaultValue, delta));
		}

		#endregion


		#region Basic overrides

		public static T Get<T>(this IMemcachedClient self, string key)
		{
			return Run(self.GetAsync<T>(key));
		}

		public static IDictionary<string, object> Get(this IMemcachedClient self, IEnumerable<string> keys)
		{
			return Run(self.GetAsync(keys));
		}


		public static bool Remove(this IMemcachedClient self, string key)
		{
			return Run(self.RemoveAsync(key));
		}

		public static bool Concate(this IMemcachedClient self, ConcatenationMode mode, string key, ArraySegment<byte> data)
		{
			return Run(self.ConcateAsync(mode, key, data));
		}


		public static ServerStats Stats(this IMemcachedClient self, string key)
		{
			return Run(self.StatsAsync(key));
		}

		public static bool FlushAll(this IMemcachedClient self)
		{
			return Run(self.FlushAllAsync());
		}
		#endregion

		public static object Get(this IMemcachedClient self, string key)
		{
			return self.Get<object>(key);
		}

		public static Task<object> GetAsync(this IMemcachedClient self, string key)
		{
			return self.GetAsync<object>(key);
		}

		#region append/prepend

		public static bool Append(this IMemcachedClient self, string key, byte[] data)
		{
			return self.Append(key, new ArraySegment<byte>(data));
		}

		public static bool Append(this IMemcachedClient self, string key, ArraySegment<byte> data)
		{
			return self.Concate(ConcatenationMode.Append, key, data);
		}

		public static bool Prepend(this IMemcachedClient self, string key, byte[] data)
		{
			return self.Prepend(key, new ArraySegment<byte>(data));
		}

		public static bool Prepend(this IMemcachedClient self, string key, ArraySegment<byte> data)
		{
			return self.Concate(ConcatenationMode.Prepend, key, data);
		}

		public static Task<bool> AppendAsync(this IMemcachedClient self, string key, byte[] data)
		{
			return self.AppendAsync(key, new ArraySegment<byte>(data));
		}

		public static Task<bool> AppendAsync(this IMemcachedClient self, string key, ArraySegment<byte> data)
		{
			return self.ConcateAsync(ConcatenationMode.Append, key, data);
		}

		public static Task<bool> PrependAsync(this IMemcachedClient self, string key, byte[] data)
		{
			return self.PrependAsync(key, new ArraySegment<byte>(data));
		}

		public static Task<bool> PrependAsync(this IMemcachedClient self, string key, ArraySegment<byte> data)
		{
			return self.ConcateAsync(ConcatenationMode.Prepend, key, data);
		}

		#endregion
		#region Add

		public static bool Add(this IMemcachedClient self, string key, object value, DateTime expiresAt)
		{
			return self.Store(StoreMode.Add, key, value, expiresAt);
		}

		public static bool Add(this IMemcachedClient self, string key, object value, TimeSpan validFor)
		{
			return self.Store(StoreMode.Add, key, value, validFor);
		}

		public static Task<bool> AddAsync(this IMemcachedClient self, string key, object value, DateTime expiresAt)
		{
			return self.StoreAsync(StoreMode.Add, key, value, expiresAt);
		}

		public static Task<bool> AddAsync(this IMemcachedClient self, string key, object value, TimeSpan validFor)
		{
			return self.StoreAsync(StoreMode.Add, key, value, validFor);
		}

		#endregion
		#region Replace

		public static bool Replace(this IMemcachedClient self, string key, object value, DateTime expiresAt)
		{
			return self.Store(StoreMode.Replace, key, value, expiresAt);
		}

		public static bool Replace(this IMemcachedClient self, string key, object value, TimeSpan validFor)
		{
			return self.Store(StoreMode.Replace, key, value, validFor);
		}

		public static Task<bool> ReplaceAsync(this IMemcachedClient self, string key, object value, DateTime expiresAt)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, expiresAt);
		}

		public static Task<bool> ReplaceAsync(this IMemcachedClient self, string key, object value, TimeSpan validFor)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, validFor);
		}

		#endregion
		#region Set

		public static bool Set(this IMemcachedClient self, string key, object value, DateTime expiresAt)
		{
			return self.Store(StoreMode.Set, key, value, expiresAt);
		}

		public static bool Set(this IMemcachedClient self, string key, object value, TimeSpan validFor)
		{
			return self.Store(StoreMode.Set, key, value, validFor);
		}

		public static Task<bool> SetAsync(this IMemcachedClient self, string key, object value, DateTime expiresAt)
		{
			return self.StoreAsync(StoreMode.Set, key, value, expiresAt);
		}

		public static Task<bool> SetAsync(this IMemcachedClient self, string key, object value, TimeSpan validFor)
		{
			return self.StoreAsync(StoreMode.Set, key, value, validFor);
		}

		#endregion
		#region Inc

		public static ulong Increment(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, DateTime expiresAt)
		{
			return self.Mutate(MutationMode.Increment, key, defaultValue, delta, expiresAt);
		}

		public static ulong Increment(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, TimeSpan validFor)
		{
			return self.Mutate(MutationMode.Increment, key, defaultValue, delta, validFor);
		}

		public static Task<ulong> IncrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, DateTime expiresAt)
		{
			return self.MutateAsync(MutationMode.Increment, key, expiresAt, defaultValue, delta);
		}

		public static Task<ulong> IncrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, TimeSpan validFor)
		{
			return self.MutateAsync(MutationMode.Increment, key, validFor, defaultValue, delta);
		}

		public static ulong Decrement(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, DateTime expiresAt)
		{
			return self.Mutate(MutationMode.Decrement, key, defaultValue, delta, expiresAt);
		}

		#endregion
		#region Decr

		public static ulong Decrement(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, TimeSpan validFor)
		{
			return self.Mutate(MutationMode.Decrement, key, defaultValue, delta, validFor);
		}

		public static Task<ulong> DecrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, DateTime expiresAt)
		{
			return self.MutateAsync(MutationMode.Decrement, key, expiresAt, defaultValue, delta);
		}

		public static Task<ulong> DecrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, TimeSpan validFor)
		{
			return self.MutateAsync(MutationMode.Decrement, key, validFor, defaultValue, delta);
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
