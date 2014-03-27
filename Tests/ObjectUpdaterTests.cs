using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Enyim.Caching.Memcached.Configuration;
using Xunit;

namespace Tests
{
	public class ObjectUpdaterTests
	{
		private static readonly int ExpectedInterval = 100;
		private static readonly TimeSpan ExpectedTimeout = TimeSpan.FromHours(10);
		private static readonly string ExpectedName = "My Name";
		private static readonly DateTime ExpectedSince = new DateTime(2013, 2, 3, 12, 24, 0);
		private static readonly bool ExpectedA = true;

		[Fact]
		public void Null_Parameters_Must_Throw()
		{
			var e = Assert.Throws<ArgumentNullException>(() => ObjectUpdater.Update(null, new Dictionary<string, string>()));
			Assert.Equal("instance", e.ParamName);

			e = Assert.Throws<ArgumentNullException>(() => ObjectUpdater.Update(new Sample(), null));
			Assert.Equal("source", e.ParamName);
		}

		[Fact]
		public void Calling_With_Empty_Dictionary_Should_Succeed()
		{
			Assert.DoesNotThrow(() => ObjectUpdater.Update(new Sample(), new Dictionary<string, string>()));
		}

		[Fact]
		public void Empty_Source_Key_Must_Fail()
		{
			Assert.Throws<InvalidOperationException>(() => ObjectUpdater.Update(new Sample(), new Dictionary<string, string>() { { "", "100" } }));
		}

		[Fact]
		public void Test_Browsable_Property()
		{
			var source = new Dictionary<string, string> { { "IAmBrowsable", "200" } };
			var sample = CreateAndUpdate(source);

			Assert.Equal(200, sample.IAmBrowsable);
		}

		[Fact]
		public void Test_Non_Browsable_Property()
		{
			var source = new Dictionary<string, string> { { "HiddenProperty", "200" } };
			ExpectMissingMember(source, source.Keys.First());
		}

		[Fact]
		public void Test_Missing_Property()
		{
			var source = new Dictionary<string, string> { { "Missing", "200" } };
			ExpectMissingMember(source, source.Keys.First());
		}

		private static void ExpectMissingMember(Dictionary<string, string> source, string memberName)
		{
			var e = Assert.Throws<MissingMemberException>(() => CreateAndUpdate(source));

			Assert.Equal(String.Format("Member '{0}.{1}' not found.", typeof(Sample).FullName, memberName), e.Message);
		}

		[Fact]
		public void Test_Exact_Names()
		{
			var source = new Dictionary<string, string>
			{
				{ "Interval", ExpectedInterval.ToString() },
				{ "Timeout", ExpectedTimeout.ToString() },
				{ "Name", ExpectedName },
				{ "Since", ExpectedSince.ToString() },
				{ "A", ExpectedA.ToString() }
			};

			CheckAll(source);
		}

		[Fact]
		public void Test_Camel_Names()
		{
			var source = new Dictionary<string, string>
			{
				{ "interval", ExpectedInterval.ToString() },
				{ "timeout", ExpectedTimeout.ToString() },
				{ "name", ExpectedName },
				{ "since", ExpectedSince.ToString() },
				{ "a", ExpectedA.ToString() }
			};

			CheckAll(source);
		}

		private static void CheckAll(IDictionary<string, string> source)
		{
			var sample = CreateAndUpdate(source);

			Assert.Equal(ExpectedInterval, sample.Interval);
			Assert.Equal(ExpectedTimeout, sample.Timeout);
			Assert.Equal(ExpectedName, sample.Name);
			Assert.Equal(ExpectedSince, sample.Since);
			Assert.Equal(ExpectedA, sample.A);
		}

		private static Sample CreateAndUpdate(IDictionary<string, string> source)
		{
			var retval = new Sample();
			ObjectUpdater.Update(retval, source);

			return retval;
		}

		private class Sample
		{
			public bool A { get; set; }
			public int Interval { get; set; }
			public TimeSpan Timeout { get; set; }
			public string Name { get; set; }
			public DateTime Since { get; set; }

			[Browsable(true)]
			public int IAmBrowsable { get; set; }

			[Browsable(false)]
			public int HiddenProperty { get; set; }
		}
	}
}
