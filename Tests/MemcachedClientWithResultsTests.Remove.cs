using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientWithResultsTests
	{
		[Fact]
		public void When_Removing_A_Valid_Key_Result_Is_Successful()
		{
			var key = GetUniqueKey("Remove_Valid");

			ShouldPass(Store(key: key));
			ShouldPass(client.Remove(key), checkCas: false);
			ShouldFail(client.Get(key));
		}

		[Fact]
		public void When_Removing_An_Invalid_Key_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Remove_Invalid");

			ShouldFail(client.Get(key)); // sanity-check
			ShouldFail(client.Remove(key));
		}
	}
}
