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
			const int ITER = 3000;
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
													.Concat(Enumerable.Repeat(NORM, queue.Count - inserts))),
							"insert 1, count: " + queue.Count);

				for (var i = 0; i < inserts; i++)
				{
					Assert.True(GUARD == queue.Dequeue(), "1st dequeue, count: " + queue.Count);
				}

				Assert.True(NORM == queue.Dequeue(), "2nd dequeue, count: " + queue.Count);
				Assert.True(NORM == queue.Dequeue(), "3nd dequeue, count: " + queue.Count);

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
		public void Merge_Data_Wrapped_To_Normal()
		{
			var src = MkDataWrappedQueue(40, Enumerable.Range(1, 20));
			var dest = new AdvQueue<int>();

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(1, 20).SequenceEqual(dest));
		}

		[Fact]
		public void Merge_Normal_To_Data_Wrapped()
		{
			var src = new AdvQueue<int>(Enumerable.Range(100, 20));
			var dest = MkDataWrappedQueue(40, Enumerable.Range(1, 20));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(1, 20).Concat(Enumerable.Range(100, 20)).SequenceEqual(dest));
		}

		[Fact]
		public void Merge_From_Data_Wrapped_To_Data_Wrapped()
		{
			var src = MkDataWrappedQueue(40, Enumerable.Range(100, 20));
			var dest = MkDataWrappedQueue(40, Enumerable.Range(200, 38));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 38).Concat(Enumerable.Range(100, 20)).SequenceEqual(dest));
		}

		[Fact]
		public void Merge_Normal_To_Space_Wrapped()
		{
			var src = new AdvQueue<int>(Enumerable.Range(100, 18));
			var dest = MkSpaceWrappedQueue(64, Enumerable.Range(1, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(1, 40).Concat(Enumerable.Range(100, 18)).SequenceEqual(dest));
		}

		[Fact]
		public void Merge_From_Space_Wrapped_To_Space_Wrapped()
		{
			var src = MkSpaceWrappedQueue(64, Enumerable.Range(100, 40));
			var dest = MkSpaceWrappedQueue(64, Enumerable.Range(200, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 40).Concat(Enumerable.Range(100, 40)).SequenceEqual(dest));
		}

		[Fact]
		public void Merge_From_Space_Wrapped_To_Data_Wrapped()
		{
			var src = MkSpaceWrappedQueue(64, Enumerable.Range(100, 40));
			var dest = MkDataWrappedQueue(64, Enumerable.Range(200, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 40).Concat(Enumerable.Range(100, 40)).SequenceEqual(dest));
		}

		[Fact]
		public void Merge_From_Data_Wrapped_To_Space_Wrapped()
		{
			var src = MkDataWrappedQueue(64, Enumerable.Range(100, 40));
			var dest = MkSpaceWrappedQueue(64, Enumerable.Range(200, 40));

			dest.Enqueue(src);
			Assert.True(Enumerable.Range(200, 40).Concat(Enumerable.Range(100, 40)).SequenceEqual(dest));
		}

		[Fact]
		public void Merge_Queues()
		{
			var range = Enumerable.Range(1, 10);
			var queue = new AdvQueue<int>(range);

			for (var i = 1; i < 5; i++)
			{
				var r2 = Enumerable.Range(i * 100, 50);
				range = range.Concat(r2);
				var tmp = new AdvQueue<int>(r2);

				queue.Enqueue(tmp);

				Assert.True(range.SequenceEqual(queue), i.ToString());
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
	}
}
