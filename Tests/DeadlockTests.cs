using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class DeadlockTest_1 : MemcachedClientTestBase, IClassFixture<ClientTestSetupFixture>
	{
		private IMemcachedClient client;

		public DeadlockTest_1(ClientTestSetupFixture data)
		{
			client = new MemcachedClient(data.ClientConfig);
		}

		[Fact]
		public void When_Storing_Item_With_New_Key_And_StoreMode_Add_Result_Is_Successful()
		{
			Assert.True(Store(StoreMode.Add, key: GetUniqueKey("Add_Once")));
		}

		[Fact]
		public void When_Storing_Item_With_Existing_Key_And_StoreMode_Add_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Add_Twice");

			Assert.True(Store(StoreMode.Add, key: key));
			Assert.False(Store(StoreMode.Add, key: key));
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}
	}

	public partial class DeadlockTest_2 : MemcachedClientTestBase, IClassFixture<ClientTestSetupFixture>
	{
		private IMemcachedClient client;

		public DeadlockTest_2(ClientTestSetupFixture data)
		{
			client = new MemcachedClient(data.ClientConfig);
		}

		[Fact]
		public void When_Storing_Item_With_New_Key_And_StoreMode_Add_Result_Is_Successful()
		{
			Assert.True(Store(StoreMode.Add, key: GetUniqueKey("Add_Once")));
		}

		[Fact]
		public void When_Storing_Item_With_Existing_Key_And_StoreMode_Add_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Add_Twice");

			Assert.True(Store(StoreMode.Add, key: key));
			Assert.False(Store(StoreMode.Add, key: key));
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}
	}

	public partial class DeadlockTest_3 : MemcachedClientTestBase, IClassFixture<ClientTestSetupFixture>
	{
		private IMemcachedClient client;

		public DeadlockTest_3(ClientTestSetupFixture data)
		{
			client = new MemcachedClient(data.ClientConfig);
		}

		[Fact]
		public void When_Storing_Item_With_New_Key_And_StoreMode_Add_Result_Is_Successful()
		{
			Assert.True(Store(StoreMode.Add, key: GetUniqueKey("Add_Once")));
		}

		[Fact]
		public void When_Storing_Item_With_Existing_Key_And_StoreMode_Add_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Add_Twice");

			Assert.True(Store(StoreMode.Add, key: key));
			Assert.False(Store(StoreMode.Add, key: key));
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}
	}

	public partial class DeadlockTest_4 : MemcachedClientTestBase, IClassFixture<ClientTestSetupFixture>
	{
		private IMemcachedClient client;

		public DeadlockTest_4(ClientTestSetupFixture data)
		{
			client = new MemcachedClient(data.ClientConfig);
		}

		[Fact]
		public void When_Storing_Item_With_New_Key_And_StoreMode_Add_Result_Is_Successful()
		{
			Assert.True(Store(StoreMode.Add, key: GetUniqueKey("Add_Once")));
		}

		[Fact]
		public void When_Storing_Item_With_Existing_Key_And_StoreMode_Add_Result_Is_Not_Successful()
		{
			var key = GetUniqueKey("Add_Twice");

			Assert.True(Store(StoreMode.Add, key: key));
			Assert.False(Store(StoreMode.Add, key: key));
		}

		protected bool Store(StoreMode mode = StoreMode.Set, string key = null, object value = null)
		{
			if (key == null) key = GetUniqueKey("store");
			if (value == null) value = GetRandomString();

			return client.Store(mode, key, value);
		}
	}
}
