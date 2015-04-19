using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using Xunit;

namespace Enyim.Caching.Tests
{
	public abstract class MemcachedClientTestBase
	{
		private readonly string name;
		private readonly Random random;

		protected MemcachedClientTestBase(string name)
		{
			this.name = name + "_";
			this.random = new Random();
		}

		protected MemcachedClientTestBase()
		{
			this.name = GetType().Name + "_";
			this.random = new Random();
		}

		protected string GetUniqueKey(string prefix = null)
		{
			return (!String.IsNullOrEmpty(prefix) ? prefix + "_" : "") + name + DateTime.Now.Ticks;
		}

		protected string[] GetUniqueKeys(string prefix = null, int max = 5)
		{
			return Enumerable
					.Range(0, max)
					.Select(i => GetUniqueKey(prefix) + "_" + i)
					.ToArray();
		}

		protected string GetRandomString()
		{
			return "unit_test_value_" + random.Next();
		}

		public async static Task<T> IfThrowsAsync<T>(Func<Task> testCode) where T : Exception
		{
			try
			{
				await testCode();
				Assert.Throws<T>(() => { });
			}
			catch (T exception)
			{
				return exception;
			}

			return null;
		}

		public async static Task<T> IfThrowsAsync<T>(Task testCode) where T : Exception
		{
			try
			{
				await testCode;
				Assert.Throws<T>(() => { });
			}
			catch (T exception)
			{
				return exception;
			}

			return null;
		}

		public static void IfThrows<T>(IOperationResult result) where T : Exception
		{
			Assert.False(result.Success);
			Assert.IsType<T>(result.Exception);
		}
	}
}
