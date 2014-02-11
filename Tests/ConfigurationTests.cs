﻿using System;
using System.Configuration;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Tests
{
	public class UnitTest1
	{
		[Fact]
		public void Can_load_client_section()
		{
			var section = System.Configuration.ConfigurationManager.GetSection("enyim.com/memcached/client") as ClientConfigurationSection;
			Assert.NotNull(section);

			Assert.Equal(typeof(TestOperationfactory), section.OperationFactory.Type);
			Assert.Equal(typeof(TestTranscoder), section.Transcoder.Type);

			//Assert.True(((TestTranscoder)section.Transcoder).IsInitialized, "Transcoder should have been initialized");
			//Assert.True(((TestOperationfactory)section.OperationFactory).IsInitialized, "Operationfactory should have been initialized");
		}
	}

	class TestKeyTransformer : IKeyTransformer, ISupportInitialize
	{
		public bool IsInitialized { get; set; }

		public void Initialize(System.Collections.Generic.IDictionary<string, string> properties)
		{
			Assert.True(properties.ContainsKey("testKey"));
			Assert.Equal(properties["testKey"], "test value");
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
			Assert.True(properties.ContainsKey("testKey"));
			Assert.Equal(properties["testKey"], "test value");
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
			Assert.True(properties.ContainsKey("testKey"));
			Assert.Equal(properties["testKey"], "test value");
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

		public IGetOperation Get(byte[] key)
		{
			throw new NotImplementedException();
		}

		public IStoreOperation Store(StoreMode mode, byte[] key, CacheItem value, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IDeleteOperation Delete(byte[] key, ulong cas)
		{
			throw new NotImplementedException();
		}

		public IMutateOperation Mutate(MutationMode mode, byte[] key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IConcatOperation Concat(ConcatenationMode mode, byte[] key, ulong cas, ArraySegment<byte> data)
		{
			throw new NotImplementedException();
		}
	}

}
