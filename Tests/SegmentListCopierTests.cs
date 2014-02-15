using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching;
using Xunit;

namespace Tests
{
	public class SegmentListCopierTests
	{
		[Fact]
		public void Write_Half_Full_Buffer()
		{
			var parts = new[]
			{
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(new byte[24]),
			};

			var target = new WriteBuffer(48);
			target.Append(new byte[24], 0, 24);

			var slc = new SegmentListCopier(parts);

			Assert.True(slc.WriteTo(target), "1"); // writes 24, remaining 96

			target.Reset();
			Assert.True(slc.WriteTo(target), "2"); // writes 48, remaining 48

			target.Reset();
			Assert.False(slc.WriteTo(target), "4"); // writes 48, remaining 0
		}

		[Fact]
		public void Handle_Empty_Segments_While_At_The_End_Of_The_Buffer()
		{
			var parts = new[]
			{
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(),
				new ArraySegment<byte>(),
			};

			var target = new WriteBuffer(48);
			target.Append(new byte[24], 0, 24);

			var slc = new SegmentListCopier(parts);

			Assert.False(slc.WriteTo(target));
		}

		[Fact]
		public void Handle_Empty_Segments_EveryWhere()
		{
			var parts = new[]
			{
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(),
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(new byte[24]),
				new ArraySegment<byte>(),
				new ArraySegment<byte>(),
				new ArraySegment<byte>(),
				new ArraySegment<byte>(new byte[24]),
			};

			var target = new WriteBuffer(48);
			var slc = new SegmentListCopier(parts);

			Assert.True(slc.WriteTo(target));
			target.Reset();
			Assert.False(slc.WriteTo(target));
		}
	}
}
