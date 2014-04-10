using System;
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
	public partial class MemcachedClientTests : MemcachedClientTestBase, IUseFixture<ClientTestSetupFixture>
	{
		private IMemcachedClient client;

		public MemcachedClientTests() : base("MemcachedClientTests") { }

		public void SetFixture(ClientTestSetupFixture data)
		{
			client = new MemcachedClient(data.ClientConfig);
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
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
