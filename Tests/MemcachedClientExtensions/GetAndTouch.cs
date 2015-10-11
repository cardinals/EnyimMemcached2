using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Moq;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientExtensionsTests
	{
		[Fact]
		public void GetAndTouchAsync_NoExpiration_NoCas()
		{
			Verify(c => c.GetAndTouchAsync<int>(Key), c => c.GetAndTouchAsync<int>(Key, Expiration.Never, NoCas));
			Verify(c => c.GetAndTouchAsync<string>(Key), c => c.GetAndTouchAsync<string>(Key, Expiration.Never, NoCas));
			Verify(c => c.GetAndTouchAsync<object>(Key), c => c.GetAndTouchAsync<object>(Key, Expiration.Never, NoCas));
			Verify(c => c.GetAndTouchAsync<DateTime>(Key), c => c.GetAndTouchAsync<DateTime>(Key, Expiration.Never, NoCas));
		}

		[Fact]
		public void GetAndTouchAsync_NoExpiration_HasCas()
		{
			Verify(c => c.GetAndTouchAsync<int>(Key, HasCas), c => c.GetAndTouchAsync<int>(Key, Expiration.Never, HasCas));
			Verify(c => c.GetAndTouchAsync<string>(Key, HasCas), c => c.GetAndTouchAsync<string>(Key, Expiration.Never, HasCas));
			Verify(c => c.GetAndTouchAsync<object>(Key, HasCas), c => c.GetAndTouchAsync<object>(Key, Expiration.Never, HasCas));
			Verify(c => c.GetAndTouchAsync<DateTime>(Key, HasCas), c => c.GetAndTouchAsync<DateTime>(Key, Expiration.Never, HasCas));
		}

		[Fact]
		public void GetAndTouchAsync_HasExpiration_NoCas()
		{
			Verify(c => c.GetAndTouchAsync<int>(Key, HasExpiration), c => c.GetAndTouchAsync<int>(Key, HasExpiration, NoCas));
			Verify(c => c.GetAndTouchAsync<string>(Key, HasExpiration), c => c.GetAndTouchAsync<string>(Key, HasExpiration, NoCas));
			Verify(c => c.GetAndTouchAsync<object>(Key, HasExpiration), c => c.GetAndTouchAsync<object>(Key, HasExpiration, NoCas));
			Verify(c => c.GetAndTouchAsync<DateTime>(Key, HasExpiration), c => c.GetAndTouchAsync<DateTime>(Key, HasExpiration, NoCas));
		}

		[Fact]
		public void GetAndTouch_NoExpiration_NoCas()
		{
			Verify(c => c.GetAndTouch<int>(Key), c => c.GetAndTouchAsync<int>(Key, Expiration.Never, NoCas));
			Verify(c => c.GetAndTouch<string>(Key), c => c.GetAndTouchAsync<string>(Key, Expiration.Never, NoCas));
			Verify(c => c.GetAndTouch<object>(Key), c => c.GetAndTouchAsync<object>(Key, Expiration.Never, NoCas));
			Verify(c => c.GetAndTouch<DateTime>(Key), c => c.GetAndTouchAsync<DateTime>(Key, Expiration.Never, NoCas));
		}

		[Fact]
		public void GetAndTouch_NoExpiration_HasCas()
		{
			Verify(c => c.GetAndTouch<int>(Key, HasCas), c => c.GetAndTouchAsync<int>(Key, Expiration.Never, HasCas));
			Verify(c => c.GetAndTouch<string>(Key, HasCas), c => c.GetAndTouchAsync<string>(Key, Expiration.Never, HasCas));
			Verify(c => c.GetAndTouch<object>(Key, HasCas), c => c.GetAndTouchAsync<object>(Key, Expiration.Never, HasCas));
			Verify(c => c.GetAndTouch<DateTime>(Key, HasCas), c => c.GetAndTouchAsync<DateTime>(Key, Expiration.Never, HasCas));
		}

		[Fact]
		public void GetAndTouch_HasExpiration_NoCas()
		{
			Verify(c => c.GetAndTouch<int>(Key, HasExpiration), c => c.GetAndTouchAsync<int>(Key, HasExpiration, NoCas));
			Verify(c => c.GetAndTouch<string>(Key, HasExpiration), c => c.GetAndTouchAsync<string>(Key, HasExpiration, NoCas));
			Verify(c => c.GetAndTouch<object>(Key, HasExpiration), c => c.GetAndTouchAsync<object>(Key, HasExpiration, NoCas));
			Verify(c => c.GetAndTouch<DateTime>(Key, HasExpiration), c => c.GetAndTouchAsync<DateTime>(Key, HasExpiration, NoCas));
		}
	}
}
