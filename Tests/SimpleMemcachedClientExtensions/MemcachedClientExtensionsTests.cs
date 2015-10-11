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
	public partial class SimpleMemcachedClientExtensionsTests
	{
		private const string Key = "MockKey";
		private const ulong MutateDelta = 16;
		private const ulong MutateDefault = 8;
		private const ulong HasCas = 100;
		private const ulong NoCas = Protocol.NO_CAS;
		private static readonly Expiration HasExpiration = new Expiration(1234);
		private static readonly ArraySegment<byte> Data = new ArraySegment<byte>(new byte[] { 1, 2, 3, 4 });
		private static readonly byte[] PlainData = new byte[] { 1, 2, 3, 4 };
		private static readonly object Value = new Object();

		private void Verify(Action<ISimpleMemcachedClient> what, Expression<Action<ISimpleMemcachedClient>> how)
		{
			var c = new Mock<ISimpleMemcachedClient>();

			what(c.Object);
			c.Verify(how);
		}

		private void Verify<TResult>(Action<ISimpleMemcachedClient> what, Expression<Func<ISimpleMemcachedClient, TResult>> how)
		{
			var c = new Mock<ISimpleMemcachedClient>();

			what(c.Object);
			c.Verify(how);
		}
	}
}

#region [ License information          ]

/* ************************************************************
 *
 *    Copyright (c) Attila Kiskó, enyim.com
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
