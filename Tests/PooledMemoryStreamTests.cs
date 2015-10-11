using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using Enyim.Caching.Memcached.Operations;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class PooledMemoryStreamTests
	{
		[Fact]
		public void Stream_Looks_OK()
		{
			var stream = new PooledMemoryStream(new BufferManagerAllocator(1024, 1024 * 1024));

			Assert.True(stream.CanSeek);
			Assert.True(stream.CanRead);
			Assert.True(stream.CanWrite);
		}

		[Fact]
		public void Write_Sets_Properties_Properly()
		{
			var sizes = new int[] { 128 + 256 + 512 + 1024 + 1024 + 2, 200, 1000, 1024 * 1024, 4 };
			var pool = new BufferManagerAllocator(1024, 1024 * 1024);

			foreach (var size in sizes)
			{
				using (var stream = new PooledMemoryStream(pool))
				{
					var data = new byte[size];

					stream.Write(data, 0, data.Length);

					Assert.Equal(data.Length, stream.Position);
					Assert.Equal(data.Length, stream.Length);
				}
			}
		}
		[Fact]
		public void Read_Must_Indicate_End_Of_Stream()
		{
			var pool = new BufferManagerAllocator(1024, 1024 * 1024);
			var stream = new PooledMemoryStream(pool);
			var buffer = new byte[129];

			Assert.Equal(-1, stream.Read(buffer, 0, buffer.Length));

			stream.Write(buffer, 0, buffer.Length);
			Assert.Equal(-1, stream.Read(buffer, 0, buffer.Length));
		}


		[Fact]
		public void Data_Written_Can_Be_Read_Back()
		{
			var sizes = new int[] { 128 + 256 + 512 + 1024 + 1024 + 2, 200, 1000, 1024 * 1024, 4 };
			var pool = new BufferManagerAllocator(1024, 1024 * 1024);
			byte i = 1;

			foreach (var size in sizes)
			{
				using (var stream = new PooledMemoryStream(pool))
				{
					var data = new byte[size];
					for (var j = 0; j < data.Length; j++) data[j] = i;

					stream.Write(data, 0, data.Length);
					stream.Position = 0;

					Assert.Equal(0, stream.Position);
					Assert.Equal(data.Length, stream.Length);

					var copy = new byte[size];
					stream.Read(copy, 0, copy.Length);
					Assert.Equal(data, copy);
				}

				i++;
			}
		}

		static void Fill(byte[] data, byte value)
		{
			for (var i = 0; i < data.Length; i++) data[i] = value;
		}

		[Fact]
		public void Segmented_Data_Written_Can_Be_Read_Back_In_Segments()
		{
			var sizes = new int[] { 128 + 256 + 512 + 1024 + 1024 + 2, 200, 1000, 1024 * 1024 };
			var pool = new BufferManagerAllocator(1024, 1024 * 1024);
			const int STEP_WRITE = 37;

			foreach (var size in sizes)
			{
				using (var stream = new PooledMemoryStream(pool))
				{
					var remaining = size;
					var data = new byte[STEP_WRITE];
					byte value = 1;

					for (var i = 0; i < size - (size % STEP_WRITE); i += STEP_WRITE)
					{
						Fill(data, value);
						stream.Write(data, 0, data.Length);
						value++;
						remaining -= STEP_WRITE;
					}

					if (remaining > 0)
					{
						Fill(data, value);
						stream.Write(data, 0, remaining);
					}

					Assert.Equal(size, stream.Position);
					Assert.Equal(size, stream.Length);

					stream.Position = 0;
					Assert.Equal(0, stream.Position);
					var copy = new byte[STEP_WRITE];

					value = 1;

					for (var i = 0; i < size - (size % STEP_WRITE); i += STEP_WRITE)
					{
						Fill(data, value);
						Fill(copy, (byte)(value - 1));
						stream.Read(copy, 0, copy.Length);
						value++;
						remaining -= STEP_WRITE;

						Assert.Equal(data, copy);
					}

					if (remaining > 0)
					{
						Fill(data, value);
						stream.Write(data, 0, remaining);
						Assert.Equal(data.Take(remaining), copy.Take(remaining));
					}
				}
			}
		}

		[Fact]
		public void Can_Rewrite_Stream()
		{
			var pool = new BufferManagerAllocator(1024, 1024 * 1024);
			var stream = new PooledMemoryStream(pool);

			var chunk = new byte[60];
			for (var i = 0; i < 10; i++)
			{
				Fill(chunk, (byte)(i + 1));
				stream.Write(chunk, 0, chunk.Length);

				Assert.Equal((i + 1) * 60, stream.Position);
				Assert.Equal((i + 1) * 60, stream.Length);
			}

			stream.Position = 120;
			Fill(chunk, 200);
			stream.Write(chunk, 0, chunk.Length);

			stream.Position = 370;
			Fill(chunk, 100);
			stream.Write(chunk, 0, chunk.Length);

			Assert.Equal(430, stream.Position);
			Assert.Equal(600, stream.Length);

			var all = new byte[stream.Length];
			stream.Position = 0;
			stream.Read(all, 0, all.Length);

			Assert.Equal(600, stream.Position);
			Assert.Equal(600, stream.Length);

			var expected = new[]
			{
				Enumerable.Repeat(1, 60), // 0
				Enumerable.Repeat(2, 60), // 60
				Enumerable.Repeat(200, 60),	// 120
				Enumerable.Repeat(4, 60), // 180
				Enumerable.Repeat(5, 60), // 240
				Enumerable.Repeat(6, 60), // 300
				Enumerable.Repeat(7, 10), // 360
				Enumerable.Repeat(100, 60),	// 370
				Enumerable.Repeat(8, 50), // 430
				Enumerable.Repeat(9, 60), // 480
				Enumerable.Repeat(10, 60), // 540
			};

			var expectedValue = expected.SelectMany(_ => _.Select(v => (byte)v)).ToArray();

			Assert.Equal(expectedValue.Length, all.Length);
			Assert.Equal(stream.Length, all.Length);

			AssertArray(expectedValue, all);
		}

		private static void AssertArray(byte[] expected, byte[] value)
		{
			Assert.Equal(expected.Length, value.Length);

			for (var i = 0; i < expected.Length; i++)
			{
				Assert.True(expected[i] == value[i], String.Format("Difference at {0}, expected {1} got {2}", i, expected[i], value[i]));
			}
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
