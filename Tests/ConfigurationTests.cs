using System;
using System.Configuration;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void Can_load_client_section()
		{
			try
			{
				var section = ConfigurationManager.GetSection("enyim.com/memcached/client") as IMemcachedClientConfiguration;
				Assert.IsNotNull("section", "Config section should be IMemcachedClientConfiguration");

				Assert.IsInstanceOfType(section.OperationFactory, typeof(TestOperationfactory), "OperationFactory should have been TestOperationfactory");
				Assert.IsInstanceOfType(section.Transcoder, typeof(TestTranscoder), "Transcoder should have been TestTranscoder");

				Assert.IsTrue(((TestTranscoder)section.Transcoder).IsInitialized, "Transcoder should have been initialized");
				Assert.IsTrue(((TestOperationfactory)section.OperationFactory).IsInitialized, "Operationfactory should have been initialized");
			}
			catch (Exception e)
			{
				Assert.Fail(e.Message);
			}
		}
	}

	class TestKeyTransformer : IKeyTransformer, ISupportInitialize
	{
		public bool IsInitialized { get; set; }

		public void Initialize(System.Collections.Generic.IDictionary<string, string> properties)
		{
			Assert.IsTrue(properties.ContainsKey("testKey"));
			Assert.AreEqual(properties["testKey"], "test value");
			IsInitialized = true;
		}

		public byte[] Transform(string key)
		{
			throw new NotImplementedException();
		}
	}

	class TestTranscoder : ITranscoder, ISupportInitialize
	{
		public bool IsInitialized { get; set; }

		public void Initialize(System.Collections.Generic.IDictionary<string, string> properties)
		{
			Assert.IsTrue(properties.ContainsKey("testKey"));
			Assert.AreEqual(properties["testKey"], "test value");
			IsInitialized = true;
		}

		public CacheItem Serialize(object value)
		{
			throw new NotImplementedException();
		}

		public object Deserialize(CacheItem item)
		{
			throw new NotImplementedException();
		}
	}

	class TestOperationfactory : IOperationFactory, ISupportInitialize
	{
		public bool IsInitialized { get; set; }

		public void Initialize(System.Collections.Generic.IDictionary<string, string> properties)
		{
			Assert.IsTrue(properties.ContainsKey("testKey"));
			Assert.AreEqual(properties["testKey"], "test value");
			IsInitialized = true;
		}

		public IGetOperation Get(string key)
		{
			throw new NotImplementedException();
		}

		public IStoreOperation Store(StoreMode mode, string key, CacheItem value, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IDeleteOperation Delete(string key, ulong cas)
		{
			throw new NotImplementedException();
		}

		public IMutateOperation Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IConcatOperation Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
		{
			throw new NotImplementedException();
		}

		public IFlushOperation Flush()
		{
			throw new NotImplementedException();
		}
	}

}
