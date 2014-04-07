using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;

namespace Enyim.Caching.Tests
{
	public abstract class MemcachedClientTestsBase
	{
		static readonly IContainer clientConfig;

		static MemcachedClientTestsBase()
		{
			const string ClusterName = "MemcachedClientTests";

			new ClusterBuilder(ClusterName).FromConfiguration().Register();
			clientConfig = new ClientConfigurationBuilder().Cluster(ClusterName).Create();
		}

		protected IMemcachedClientWithResults _Client;

		protected MemcachedClientTestsBase()
		{
			_Client = new MemcachedClientWithResults(clientConfig);
		}

		protected string GetUniqueKey(string prefix = null)
		{
			return (!string.IsNullOrEmpty(prefix) ? prefix + "_" : "") +
				"unit_test_" + DateTime.Now.Ticks;
		}

		protected IEnumerable<string> GetUniqueKeys(string prefix = null, int max = 5)
		{
			var keys = new List<string>(max);
			for (int i = 0; i < max; i++)
			{
				keys.Add(GetUniqueKey(prefix));
			}

			return keys;
		}

		protected string GetRandomString()
		{
			var rand = new Random((int)DateTime.Now.Ticks).Next();
			return "unit_test_value_" + rand;
		}

		protected IOperationResult Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return _Client.Store(mode, key, value);
		}

		protected void ShouldPass(IOperationResult result)
		{
			Assert.True(result.Success, "Success was false");
			Assert.True(result.Cas > 0, "Cas value was 0");
			Assert.True(result.StatusCode == 0, "StatusCode was not 0");
		}

		protected void ShouldFail(IOperationResult result)
		{
			Assert.False(result.Success, "Success was true");
			Assert.True(result.Cas == 0, "Cas value was not 0");
			Assert.True(result.StatusCode > 0, "StatusCode not greater than 0");
			//Assert.True(result.InnerResult != null, "InnerResult was null");
		}

		protected void ShouldPass(IGetOperationResult<object> result, object expectedValue)
		{
			ShouldPass<object>(result, expectedValue);
		}

		protected void ShouldPass<T>(IGetOperationResult<T> result, T expectedValue)
		{
			ShouldPass((IOperationResult)result);

			Assert.True(result.HasValue);
			Assert.Equal(expectedValue, result.Value);
		}

		protected void ShouldFail<T>(IGetOperationResult<T> result)
		{
			ShouldFail((IOperationResult)result);

			Assert.False(result.HasValue);
			Assert.Equal(default(T), result.Value);
		}

		protected void ShoudPass(IMutateOperationResult result, ulong expectedValue)
		{
			ShouldPass((IOperationResult)result);

			Assert.Equal(expectedValue, result.Value);
		}

		//protected void MutateAssertFail(IMutateOperationResult result)
		//{
		//	Assert.That(result.Success, Is.False, "Success was true");
		//	Assert.That(result.Cas, Is.EqualTo(0), "Cas 0");
		//	Assert.That(result.StatusCode, Is.Null.Or.Not.EqualTo(0), "StatusCode was 0");
		//}

		//protected void ConcatAssertPass(IConcatOperationResult result)
		//{
		//	Assert.That(result.Success, Is.True, "Success was false");
		//	Assert.That(result.Cas, Is.GreaterThan(0), "Cas value was 0");
		//	Assert.That(result.StatusCode, Is.EqualTo(0), "StatusCode was not 0");
		//}

		//protected void ConcatAssertFail(IConcatOperationResult result)
		//{
		//	Assert.That(result.Success, Is.False, "Success was true");
		//	Assert.That(result.Cas, Is.EqualTo(0), "Cas value was not 0");
		//	Assert.That(result.StatusCode, Is.Null.Or.GreaterThan(0), "StatusCode not greater than 0");
		//}
	}
}

#region [ License information          ]
/* ************************************************************
 * 
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2012 Attila Kiskó, enyim.com
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