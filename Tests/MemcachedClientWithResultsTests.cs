﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Enyim.Caching.Memcached.Results;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsTests : MemcachedClientTestBase, IUseFixture<ClientTestSetupFixture>
	{
		private IMemcachedClientWithResults client;

		public MemcachedClientWithResultsTests() : base("MemcachedClientWithResultsTests") { }

		public void SetFixture(ClientTestSetupFixture data)
		{
			client = new MemcachedClientWithResults(data.ClientConfig);
		}

		protected IOperationResult Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}

		protected IOperationResult ShouldPass(IOperationResult result, bool checkCas = true)
		{
			Assert.True(result.Success, "Success was false");
			Assert.True(result.StatusCode == 0, "StatusCode was not 0");

			if (checkCas) Assert.True(result.Cas > 0, "Cas value was 0");

			return result;
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
			ShouldPass(result);

			Assert.True(result.HasValue);
			Assert.Equal(expectedValue, result.Value);
		}

		protected void ShouldFail<T>(IGetOperationResult<T> result)
		{
			ShouldFail((IOperationResult)result);

			Assert.False(result.HasValue);
			Assert.Equal(default(T), result.Value);
		}

		protected void ShouldFail<T>(IGetOperationResult<T> result, T shouldNotBe)
		{
			ShouldFail((IOperationResult)result);

			Assert.NotEqual(shouldNotBe, result.Value);
		}

		protected void ShouldPass(IMutateOperationResult result, ulong expectedValue)
		{
			ShouldPass(result);

			Assert.Equal(expectedValue, result.Value);
		}
	}
}

#region [ License information          ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @author Attila Kiskó <a@enyim.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2014 Attila Kiskó, enyim.com
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