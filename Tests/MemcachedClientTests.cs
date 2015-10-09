using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientTests : MemcachedClientTestBase, IClassFixture<PrivateServerFixture>
	{
		private IMemcachedClient client;

		public MemcachedClientTests(PrivateServerFixture data)
			: base("MemcachedClientTests")
		{
			client = new MemcachedClient(data.ClientConfig);
		}

		protected Task<bool> Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.StoreAsync(mode, key, value, Expiration.Never);
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
