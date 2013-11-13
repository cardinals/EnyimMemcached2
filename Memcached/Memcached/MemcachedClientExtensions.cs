using System;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public static class MemcachedClientExtensions
	{
		public static bool Append(this IMemcachedClient self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.Concate(ConcatenationMode.Append, key, data, cas);
		}

		public static bool Prepend(this IMemcachedClient self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.Concate(ConcatenationMode.Prepend, key, data, cas);
		}

		public static ulong Increment(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.Mutate(MutationMode.Increment, key, defaultValue, delta, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static ulong Increment(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.Mutate(MutationMode.Increment, key, defaultValue, delta, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static ulong Decrement(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.Mutate(MutationMode.Decrement, key, defaultValue, delta, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static ulong Decrement(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.Mutate(MutationMode.Decrement, key, defaultValue, delta, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static Task<bool> AppendAsync(this IMemcachedClient self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.ConcateAsync(ConcatenationMode.Append, key, data, cas);
		}

		public static Task<bool> PrependAsync(this IMemcachedClient self, string key, ArraySegment<byte> data, ulong cas = 0)
		{
			return self.ConcateAsync(ConcatenationMode.Prepend, key, data, cas);
		}

		public static Task<ulong> IncrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.MutateAsync(MutationMode.Increment, key, defaultValue, delta, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static Task<ulong> IncrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.MutateAsync(MutationMode.Increment, key, defaultValue, delta, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static Task<ulong> DecrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.MutateAsync(MutationMode.Decrement, key, defaultValue, delta, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static Task<ulong> DecrementAsync(this IMemcachedClient self, string key, ulong defaultValue, ulong delta, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.MutateAsync(MutationMode.Decrement, key, defaultValue, delta, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static bool Add(this IMemcachedClient self, string key, object value, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.Store(StoreMode.Add, key, value, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static bool Add(this IMemcachedClient self, string key, object value, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.Store(StoreMode.Add, key, value, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static Task<bool> AddAsync(this IMemcachedClient self, string key, object value, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.StoreAsync(StoreMode.Add, key, value, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static Task<bool> AddAsync(this IMemcachedClient self, string key, object value, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.StoreAsync(StoreMode.Add, key, value, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static bool Replace(this IMemcachedClient self, string key, object value, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.Store(StoreMode.Replace, key, value, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static bool Replace(this IMemcachedClient self, string key, object value, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.Store(StoreMode.Replace, key, value, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static Task<bool> ReplaceAsync(this IMemcachedClient self, string key, object value, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static Task<bool> ReplaceAsync(this IMemcachedClient self, string key, object value, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.StoreAsync(StoreMode.Replace, key, value, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static bool Set(this IMemcachedClient self, string key, object value, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.Store(StoreMode.Set, key, value, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static bool Set(this IMemcachedClient self, string key, object value, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.Store(StoreMode.Set, key, value, cas, validFor ?? TimeSpan.MaxValue);
		}

		public static Task<bool> SetAsync(this IMemcachedClient self, string key, object value, ulong cas = 0, DateTime? expiresAt = null)
		{
			return self.StoreAsync(StoreMode.Set, key, value, cas, expiresAt ?? DateTime.MaxValue);
		}

		public static Task<bool> SetAsync(this IMemcachedClient self, string key, object value, ulong cas = 0, TimeSpan? validFor = null)
		{
			return self.StoreAsync(StoreMode.Set, key, value, cas, validFor ?? TimeSpan.MaxValue);
		}
	}
}
