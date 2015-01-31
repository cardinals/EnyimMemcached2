using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class AdvQueueTests
	{
		[Fact]
		public void Enqueued_Items_Have_Proper_Order()
		{
			var queue = new AdvQueue<int>();
			var src = Enumerable.Range(1, 10).ToArray();

			foreach (var i in src) queue.Enqueue(i);
			Assert.True(queue.SequenceEqual(src));
		}

		[Fact]
		public void Dequeued_Items_Have_Proper_Order()
		{
			var queue = new AdvQueue<int>();
			var src = Enumerable.Range(1, 10).ToArray();

			foreach (var i in src) queue.Enqueue(i);

			var list = new List<int>();
			foreach (var i in src) list.Add(queue.Dequeue());

			Assert.True(list.SequenceEqual(src));
		}

		[Fact]
		public void Inserting_Does_Not_Mess_Up_Ordering()
		{
			var queue = new AdvQueue<int>();

			EnqueueRange(queue, Enumerable.Range(1, 4));
			Assert.True(queue.SequenceEqual(Enumerable.Range(1, 4)), "initial check");

			queue.Insert(0);
			Assert.True(queue.SequenceEqual(Enumerable.Range(0, 5)), "after insert");

			for (var i = 1; i <= 4; i++)
			{
				queue.Dequeue();
				Assert.True(queue.SequenceEqual(Enumerable.Range(i, 4 - i + 1)), "dequeue " + i);
			}
		}

		[Fact]
		public void Stress_Test_Inserts()
		{
			const int ITER = 5000;
			const int NORM = 2;
			const int GUARD = 254;

			var queue = new AdvQueue<int>(Enumerable.Repeat(NORM, ITER));
			Assert.True(queue.SequenceEqual(Enumerable.Repeat(NORM, ITER)), "initial check");

			var inserts = 1;

			while (queue.Count > 0)
			{
				for (var i = 0; i < inserts; i++) queue.Insert(GUARD);

				Assert.True(queue.SequenceEqual(Enumerable
													.Repeat(GUARD, inserts)
													.Concat(Enumerable.Repeat(NORM, queue.Count - inserts))));

				for (var i = 0; i < inserts; i++)
				{
					Assert.True(GUARD == queue.Dequeue());
				}

				Assert.True(NORM == queue.Dequeue());
				Assert.True(NORM == queue.Dequeue());

				inserts++;
			}
		}

		[Fact]
		public void Insert_Check_Bounds()
		{
			var queue = new AdvQueue<int>();

			EnqueueRange(queue, Enumerable.Repeat(1, 3));
			Assert.True(queue.SequenceEqual(Enumerable.Repeat(1, 3)), "initial check");

			queue.Insert(8);
			queue.Insert(8);
			queue.Insert(8);
			queue.Insert(8);
			queue.Insert(8);

			queue.Enqueue(2);
			queue.Enqueue(2);
			queue.Enqueue(2);
			queue.Enqueue(2);

			queue.Insert(14);
			queue.Insert(13);
			queue.Insert(12);
			queue.Insert(11);

			var expected = new[]
			{
				Enumerable.Range(11, 4),
				Enumerable.Repeat(8, 5),
				Enumerable.Repeat(1, 3),
				Enumerable.Repeat(2, 4),
			}.SelectMany(a => a);

			Assert.True(queue.SequenceEqual(expected), "final");
		}

		[Fact]
		public void Enqueing_A_Queue_Into_Another_Should_Empty_The_Original()
		{
			var one = new AdvQueue<int>(Enumerable.Repeat(1, 10));
			var other = new AdvQueue<int>(Enumerable.Repeat(2, 10));

			one.Enqueue(other);

			Assert.Equal(0, other.Count);
			Assert.Equal(20, one.Count);
		}

		[Fact]
		public void Enqueue_Data_Wrapped_To_Normal()
		{
			var src = MkDataWrappedQueue(40, Enumerable.Range(1, 20));
			var dest = new AdvQueue<int>();

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(1, 20).SequenceEqual(dest));
		}

		[Fact]
		public void Enqueue_Normal_To_Data_Wrapped()
		{
			var src = new AdvQueue<int>(Enumerable.Range(100, 20));
			var dest = MkDataWrappedQueue(40, Enumerable.Range(1, 20));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(1, 20).Concat(Enumerable.Range(100, 20)).SequenceEqual(dest));
		}

		[Fact]
		public void Enqueue_From_Data_Wrapped_To_Data_Wrapped()
		{
			var src = MkDataWrappedQueue(40, Enumerable.Range(100, 20));
			var dest = MkDataWrappedQueue(40, Enumerable.Range(200, 38));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 38).Concat(Enumerable.Range(100, 20)).SequenceEqual(dest));
		}

		[Fact]
		public void Enqueue_Normal_To_Space_Wrapped()
		{
			var src = new AdvQueue<int>(Enumerable.Range(100, 18));
			var dest = MkSpaceWrappedQueue(64, Enumerable.Range(1, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(1, 40).Concat(Enumerable.Range(100, 18)).SequenceEqual(dest));
		}

		[Fact]
		public void Enqueue_From_Space_Wrapped_To_Space_Wrapped()
		{
			var src = MkSpaceWrappedQueue(64, Enumerable.Range(100, 40));
			var dest = MkSpaceWrappedQueue(64, Enumerable.Range(200, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 40).Concat(Enumerable.Range(100, 40)).SequenceEqual(dest));
		}

		[Fact]
		public void Enqueue_From_Space_Wrapped_To_Data_Wrapped()
		{
			var src = MkSpaceWrappedQueue(64, Enumerable.Range(100, 40));
			var dest = MkDataWrappedQueue(64, Enumerable.Range(200, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 40).Concat(Enumerable.Range(100, 40)).SequenceEqual(dest));
		}

		[Fact]
		public void Enqueue_From_Data_Wrapped_To_Space_Wrapped()
		{
			var src = MkDataWrappedQueue(64, Enumerable.Range(100, 40));
			var dest = MkSpaceWrappedQueue(64, Enumerable.Range(200, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 40).Concat(Enumerable.Range(100, 40)).SequenceEqual(dest));
		}

		[Fact]
		public void Stress_Enqueue_Queues()
		{
			var range = Enumerable.Range(1, 10);
			var queue = new AdvQueue<int>(range);

			for (var i = 1; i < 50; i++)
			{
				var r2 = Enumerable.Range(i * 100, 50);
				range = range.Concat(r2);
				var tmp = new AdvQueue<int>(r2);

				queue.Enqueue(tmp);

				Assert.True(range.SequenceEqual(queue), i.ToString());
			}
		}

		[Fact]
		public void Stress_Enqueue_Using_The_Same_Target()
		{
			const int VALUE = 1234;

			var source = new AdvQueue<int>();
			var target = new AdvQueue<int>();
			var itemCount = 10;

			for (var i = 0; i < 5000; i++)
			{
				for (var q = 0; q < itemCount; q++)
					source.Enqueue(VALUE);

				target.Enqueue(source);

				for (var d = 0; d < itemCount; d++)
				{
					Assert.Equal(VALUE, target.Peek());
					Assert.Equal(VALUE, target.Dequeue());
				}

				itemCount++;
			}
		}

		private void EnqueueRange<T>(AdvQueue<T> q, IEnumerable<T> what)
		{
			foreach (var i in what) q.Enqueue(i);
		}

		/// <summary>
		/// Creates a queue where data wraps around in the array. (Head > Tail)
		/// </summary>
		private AdvQueue<int> MkDataWrappedQueue(int count, IEnumerable<int> data)
		{
			var retval = new AdvQueue<int>();
			var newData = data.ToArray();
			var capacity = (int)Math.Pow(2, Math.Ceiling(Math.Log(count, 2)));

			var toQueue = capacity - (newData.Length / 2) + 1;

			EnqueueRange(retval, Enumerable.Repeat(1, toQueue));

			for (var i = 0; i < toQueue; i++)
				retval.Dequeue();

			EnqueueRange(retval, newData);

			return retval;
		}

		[Fact]
		public void Enqueing_Into_Queue_With_Shifted_Head_Should_Move_Data_To_The_Front()
		{
			var r1 = Enumerable.Range(1, 10);
			// data.length will be 13
			// 4 -> 6 -> 9 -> 13
			var source = new AdvQueue<int>(r1);

			source.Dequeue();
			source.Dequeue();
			source.Dequeue();
			source.Dequeue();

			var r2 = Enumerable.Range(20, 4);
			source.Enqueue(new AdvQueue<int>(r2));

			Assert.Equal(source.AsEnumerable(), r1.Skip(4).Concat(r2));
		}

		[Fact]
		public void Dequeuing_An_Empty_Queue_Should_Fail()
		{
			Assert.Throws<InvalidOperationException>(() => new AdvQueue<int>().Dequeue());
		}

		[Fact]
		public void Peeking_An_Empty_Queue_Should_Fail()
		{
			Assert.Throws<InvalidOperationException>(() => new AdvQueue<int>().Peek());
		}

		[Fact]
		public void TryPeek_Should_Work()
		{
			int tmp;
			var q = new AdvQueue<int>();

			Assert.False(q.TryPeek(out tmp));

			q.Enqueue(1234);
			Assert.True(q.TryPeek(out tmp));
			Assert.Equal(1234, tmp);
		}

		/// <summary>
		/// Creates a queue where empty space wraps around in the array. (Tail > Head)
		/// </summary>
		private AdvQueue<int> MkSpaceWrappedQueue(int count, IEnumerable<int> data)
		{
			var retval = new AdvQueue<int>();
			var newData = data.ToArray();
			var capacity = (int)Math.Pow(2, Math.Ceiling(Math.Log(count, 2)));

			// grow the queue to the final capacity
			EnqueueRange(retval, Enumerable.Repeat(1, capacity / 2 + 1));
			retval.Clear();

			// move the head to a position where the newData will be in the middle of the queue
			var toQueue = (capacity - newData.Length) / 2;
			EnqueueRange(retval, Enumerable.Repeat(1, toQueue));
			for (var i = 0; i < toQueue; i++) retval.Dequeue();

			// add the data
			EnqueueRange(retval, newData);

			return retval;
		}

		[Fact]
		public void Enumeration_Works()
		{
			const int VALUE = 12;

			var queue = new AdvQueue<int>();
			var range = Enumerable.Repeat(VALUE, 100).ToArray();

			EnqueueRange(queue, range);
			Assert.Equal(queue, range);

			var c = 0;
			foreach (var i in queue)
			{
				Assert.Equal(VALUE, i);
				c++;
			}

			Assert.Equal(range.Length, c);
		}

		[Fact]
		public void Enumeration_Works_In_Wrapped_Queues()
		{
			var range = Enumerable.Range(1, 100).ToArray();

			var queue = MkSpaceWrappedQueue(100, range);
			Assert.Equal(queue, range);

			queue = MkDataWrappedQueue(100, range);
			Assert.Equal(queue, range);
		}
	}
}
