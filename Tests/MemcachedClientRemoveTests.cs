using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Tests
{
	public class MemcachedClientRemoveTests : MemcachedClientTestsBase
	{
		[Fact]
		public void When_Removing_A_Valid_Key_Result_Is_Successful()
		{
			var key = GetUniqueKey("Remove_Valid");

			ShouldPass(Store(key: key));
			ShouldPass(_Client.Remove(key), checkCas: false);
			ShouldFail(_Client.Get(key));
		}

		[Fact]
		public void When_Removing_An_Invalid_Key_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Remove_Invalid");

			ShouldFail(_Client.Get(key)); // sanity-check
			ShouldFail(_Client.Remove(key));
		}
	}
}
