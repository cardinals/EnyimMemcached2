using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;
using Moq;
using Xunit;

namespace Enyim.Caching.Tests
{
	public partial class MemcachedClientExtensionsTests
	{
		private const string Key = "MockKey";
		private const ulong Delta = 16;
		private const ulong Default = 8;
		private const ulong HasCas = 100;
		private const ulong NoCas = Protocol.NO_CAS;
		private static readonly Expiration HasExpiration = new Expiration(1234);
		private static readonly ArraySegment<byte> Data = new ArraySegment<byte>(new byte[] { 1, 2, 3, 4 });
		private static readonly byte[] PlainData = new byte[] { 1, 2, 3, 4 };
		private static readonly object Value = new Object();

		private void Verify(Action<IMemcachedClient> what, Expression<Action<IMemcachedClient>> how)
		{
			var c = new Mock<IMemcachedClient>();

			what(c.Object);
			c.Verify(how);
		}

		private void Verify<TResult>(Action<IMemcachedClient> what, Expression<Func<IMemcachedClient, TResult>> how)
		{
			var c = new Mock<IMemcachedClient>();

			what(c.Object);
			c.Verify(how);
		}
	}
}
