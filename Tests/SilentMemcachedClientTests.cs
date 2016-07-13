using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class SilentMemcachedClientTests : MemcachedClientTests, IClassFixture<SilentMemcachedClientConfigFixture>
	{
		public SilentMemcachedClientTests(SilentMemcachedClientConfigFixture fixture) : base("SilentMemcachedClientTests", fixture.Config) { }

		[Fact]
		public async void When_Incrementing_Value_Result_Is_Successful()
		{
			var key = GetUniqueKey("Increment");

			ShouldPass(await client.MutateAsync(MutationMode.Increment, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual("200", await client.GetAsync<string>(key));
		}

		[Fact]
		public async void When_Decrementing_Value_Result_Is_Successful()
		{
			var key = GetUniqueKey("Decrement");

			ShouldPass(await client.MutateAsync(MutationMode.Decrement, key, Expiration.Never, 10, 200, Protocol.NO_CAS));
			AreEqual("200", await client.GetAsync<string>(key));
		}

		[Fact]
		public async Task When_Only_Sending_Silent_Commands_All_Should_Be_Executed()
		{
			const int COUNT = 10000;

			var prefix = GetUniqueKey("StressTest");
			var tasks = Enumerable.Range(1, COUNT).Select(i => client.SetAsync(prefix + i, i)).ToArray();

			await Task.WhenAll(tasks);

			Assert.All(tasks, t => Assert.True(t.Result.Silent && t.Result.Success));
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
