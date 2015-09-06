using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class ExpirationTests
	{
		[Fact]
		public void TimeSpan_Zero_Should_Never_Expire()
		{
			var e = Expiration.Create(TimeSpan.Zero);

			Assert.Equal(0u, e.Value);
			Assert.Equal(true, e.IsAbsolute);
			Assert.Equal(true, e.IsForever);
		}

		[Fact]
		public void TimeSpan_MaxValue_Should_Never_Expire()
		{
			var e = Expiration.Create(TimeSpan.MaxValue);

			Assert.Equal(0u, e.Value);
			Assert.Equal(true, e.IsAbsolute);
			Assert.Equal(true, e.IsForever);
		}

		[Fact]
		public void TimeSpan_Less_Than_One_Month_Should_Become_Valid_Relative_Expiration()
		{
			var e = Expiration.Create(TimeSpan.FromSeconds(100));

			Assert.Equal(100u, e.Value);
			Assert.Equal(false, e.IsAbsolute);
			Assert.Equal(false, e.IsForever);
		}

		[Fact]
		public void TimeSpan_Greater_Than_One_Month_Should_Become_Valid_Absolute_Expiration()
		{
			using (SystemTime.Set(() => new DateTime(2011, 12, 31, 23, 0, 0, DateTimeKind.Utc)))
			{
				var e = Expiration.Create(TimeSpan.FromDays(31));

				Assert.Equal(1328050800u, e.Value);
				Assert.Equal(true, e.IsAbsolute);
				Assert.Equal(false, e.IsForever);
			}
		}

		[Fact]
		public void Unix_Epoch_Should_Fail()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => { var a = (Expiration)new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); });
		}

		[Fact]
		public void DateTime_MinValue_Should_Never_Expire()
		{
			var e = Expiration.Create(DateTime.MaxValue);

			Assert.Equal(0u, e.Value);
			Assert.Equal(true, e.IsAbsolute);
			Assert.Equal(true, e.IsForever);
		}

		[Fact]
		public void DateTime_MaxValue_Should_Never_Expire()
		{
			var e = Expiration.Create(DateTime.MaxValue);

			Assert.Equal(0u, e.Value);
			Assert.Equal(true, e.IsAbsolute);
			Assert.Equal(true, e.IsForever);
		}

		[Fact]
		public void DateTime_Value_Should_Become_Valid_Absolute_Expiration()
		{
			var e = Expiration.Create(new DateTime(2012, 01, 31, 23, 0, 0, DateTimeKind.Utc));

			Assert.Equal(1328050800u, e.Value);
			Assert.Equal(true, e.IsAbsolute);
			Assert.Equal(false, e.IsForever);
		}

		[Fact]
		public void Different_Instances_From_Same_DateTime_Must_Be_Equal()
		{
			var a = Expiration.Create(new DateTime(2012, 01, 31, 23, 0, 0, DateTimeKind.Utc));
			var b = (Expiration)(new DateTime(2012, 01, 31, 23, 0, 0, DateTimeKind.Utc));

			Assert.True(a.Equals(a));
			Assert.True(b.Equals(b));

			Assert.True(a.Equals(b));
			Assert.True(b.Equals(a));
		}

		[Fact]
		public void Different_Instances_From_Same_TimeSpan_Must_Be_Equal()
		{
			var a = Expiration.Create(TimeSpan.FromSeconds(100));
			var b = (Expiration)(TimeSpan.FromSeconds(100));

			Assert.True(a.Equals(a));
			Assert.True(b.Equals(b));

			Assert.True(a.Equals(b));
			Assert.True(b.Equals(a));
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
