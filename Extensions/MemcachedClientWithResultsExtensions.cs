using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public static class MemcachedClientWithResultsExtensions
	{
		private const ulong IGNORE_CAS = 0;

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

		#region Basic overrides

		public static IGetOperationResult<T> Get<T>(this IMemcachedClientWithResults self, string key)
		{
			try
			{
				return self.GetAsync<T>(key).Result;
			}
			catch (AggregateException ae)
			{
				if (ae.InnerExceptions.Count == 1) throw ae.InnerExceptions[0];
				else throw ae.Flatten();
			}
		}

		public static IDictionary<string, IGetOperationResult<object>> Get(this IMemcachedClientWithResults self, IEnumerable<string> keys)
		{
			return Run(self.GetAsync(keys));
		}

		public static IGetOperationResult<T> GetAndTouch<T>(this IMemcachedClientWithResults self, string key, DateTime expiresAt)
		{
			return Run(self.GetAndTouchAsync<T>(key, expiresAt));
		}

		public static IGetOperationResult<T> GetAndTouch<T>(this IMemcachedClientWithResults self, string key, TimeSpan validFor)
		{
			return Run(self.GetAndTouchAsync<T>(key, validFor));
		}

		public static IOperationResult Touch(this IMemcachedClientWithResults self, string key, DateTime expiresAt)
		{
			return Run(self.TouchAsync(key, expiresAt));
		}

		public static IOperationResult Touch(this IMemcachedClientWithResults self, string key, TimeSpan validFor)
		{
			return Run(self.TouchAsync(key, validFor));
		}

		public static IOperationResult Store(this IMemcachedClientWithResults self, StoreMode mode, string key, object value, DateTime expiresAt)
		{
			return Run(self.StoreAsync(mode, key, value, expiresAt, IGNORE_CAS));
		}

		public static IOperationResult Store(this IMemcachedClientWithResults self, StoreMode mode, string key, object value, TimeSpan validFor)
		{
			return Run(self.StoreAsync(mode, key, value, validFor, IGNORE_CAS));
		}

		public static IOperationResult Remove(this IMemcachedClientWithResults self, string key)
		{
			return Run(self.RemoveAsync(key, IGNORE_CAS));
		}

		public static IOperationResult Concate(this IMemcachedClientWithResults self, ConcatenationMode mode, string key, ArraySegment<byte> data, ulong cas = IGNORE_CAS)
		{
			return Run(self.ConcateAsync(mode, key, data, cas));
		}

		public static IMutateOperationResult Mutate(this IMemcachedClientWithResults self, MutationMode mode, string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return Run(self.MutateAsync(mode, key, expiresAt, defaultValue, delta, cas));
		}

		public static IMutateOperationResult Mutate(this IMemcachedClientWithResults self, MutationMode mode, string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return Run(self.MutateAsync(mode, key, validFor, defaultValue, delta, cas));
		}

		public static IStatsOperationResult Stats(this IMemcachedClientWithResults self, string key)
		{
			return Run(self.StatsAsync(key));
		}

		public static IOperationResult FlushAll(this IMemcachedClientWithResults self)
		{
			return Run(self.FlushAllAsync());
		}

		#endregion

		public static IGetOperationResult<object> Get(this IMemcachedClientWithResults self, string key)
		{
			return self.Get<object>(key);
		}

		public static Task<IGetOperationResult<object>> GetAsync(this IMemcachedClientWithResults self, string key)
		{
			return self.GetAsync<object>(key);
		}

		#region Add

		public static IOperationResult Add(this IMemcachedClientWithResults self, string key, object value, DateTime expiresAt)
		{
			return self.Store(StoreMode.Add, key, value, expiresAt);
		}

		public static IOperationResult Add(this IMemcachedClientWithResults self, string key, object value, TimeSpan validFor)
		{
			return self.Store(StoreMode.Add, key, value, validFor);
		}

		public static Task<IOperationResult> AddAsync(this IMemcachedClientWithResults self, string key, object value, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return self.StoreAsync(StoreMode.Add, key, value, expiresAt, cas);
		}

		public static Task<IOperationResult> AddAsync(this IMemcachedClientWithResults self, string key, object value, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return self.StoreAsync(StoreMode.Add, key, value, validFor, cas);
		}

		#endregion
		#region Replace

		public static IOperationResult Replace(this IMemcachedClientWithResults self, string key, object value, DateTime expiresAt)
		{
			return self.Store(StoreMode.Replace, key, value, expiresAt);
		}

		public static IOperationResult Replace(this IMemcachedClientWithResults self, string key, object value, TimeSpan validFor)
		{
			return self.Store(StoreMode.Replace, key, value, validFor);
		}

		public static Task<IOperationResult> ReplaceAsync(this IMemcachedClientWithResults self, string key, object value, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, expiresAt, cas);
		}

		public static Task<IOperationResult> ReplaceAsync(this IMemcachedClientWithResults self, string key, object value, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, validFor, cas);
		}

		#endregion
		#region Set

		public static IOperationResult Set(this IMemcachedClientWithResults self, string key, object value, DateTime expiresAt)
		{
			return self.Store(StoreMode.Set, key, value, expiresAt);
		}

		public static IOperationResult Set(this IMemcachedClientWithResults self, string key, object value, TimeSpan validFor)
		{
			return self.Store(StoreMode.Set, key, value, validFor);
		}

		public static Task<IOperationResult> SetAsync(this IMemcachedClientWithResults self, string key, object value, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return self.StoreAsync(StoreMode.Set, key, value, expiresAt, cas);
		}

		public static Task<IOperationResult> SetAsync(this IMemcachedClientWithResults self, string key, object value, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return self.StoreAsync(StoreMode.Set, key, value, validFor, cas);
		}

		#endregion

		#region append/prepend

		public static IOperationResult Append(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = IGNORE_CAS)
		{
			return self.Concate(ConcatenationMode.Append, key, new ArraySegment<byte>(data), cas);
		}

		public static IOperationResult Append(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = IGNORE_CAS)
		{
			return self.Concate(ConcatenationMode.Append, key, data, cas);
		}

		public static Task<IOperationResult> AppendAsync(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = IGNORE_CAS)
		{
			return self.ConcateAsync(ConcatenationMode.Append, key, new ArraySegment<byte>(data), cas);
		}

		public static Task<IOperationResult> AppendAsync(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = IGNORE_CAS)
		{
			return self.ConcateAsync(ConcatenationMode.Append, key, data, cas);
		}

		public static IOperationResult Prepend(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = IGNORE_CAS)
		{
			return self.Concate(ConcatenationMode.Prepend, key, new ArraySegment<byte>(data), cas);
		}

		public static IOperationResult Prepend(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = IGNORE_CAS)
		{
			return self.Concate(ConcatenationMode.Prepend, key, data, cas);
		}

		public static Task<IOperationResult> PrependAsync(this IMemcachedClientWithResults self, string key, byte[] data, ulong cas = IGNORE_CAS)
		{
			return self.ConcateAsync(ConcatenationMode.Prepend, key, new ArraySegment<byte>(data), cas);
		}

		public static Task<IOperationResult> PrependAsync(this IMemcachedClientWithResults self, string key, ArraySegment<byte> data, ulong cas = IGNORE_CAS)
		{
			return self.ConcateAsync(ConcatenationMode.Prepend, key, data, cas);
		}

		#endregion
		#region Inc

		public static IMutateOperationResult Increment(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return self.Mutate(MutationMode.Increment, key, defaultValue, delta, expiresAt, cas);
		}

		public static IMutateOperationResult Increment(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return self.Mutate(MutationMode.Increment, key, defaultValue, delta, validFor, cas);
		}

		public static Task<IMutateOperationResult> IncrementAsync(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return self.MutateAsync(MutationMode.Increment, key, expiresAt, defaultValue, delta, cas);
		}

		public static Task<IMutateOperationResult> IncrementAsync(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return self.MutateAsync(MutationMode.Increment, key, validFor, defaultValue, delta, cas);
		}

		#endregion
		#region Dec

		public static IMutateOperationResult Decrement(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return self.Mutate(MutationMode.Decrement, key, defaultValue, delta, expiresAt, cas);
		}

		public static IMutateOperationResult Decrement(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return self.Mutate(MutationMode.Decrement, key, defaultValue, delta, validFor, cas);
		}

		public static Task<IMutateOperationResult> DecrementAsync(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas = IGNORE_CAS)
		{
			return self.MutateAsync(MutationMode.Decrement, key, expiresAt, defaultValue, delta, cas);
		}

		public static Task<IMutateOperationResult> DecrementAsync(this IMemcachedClientWithResults self, string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas = IGNORE_CAS)
		{
			return self.MutateAsync(MutationMode.Decrement, key, validFor, defaultValue, delta, cas);
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
