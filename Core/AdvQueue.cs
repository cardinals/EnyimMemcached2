using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Enyim.Caching
{
	/// <summary>
	///  A queue which allows to insert items to the beginning, and also supports efficient enqueuing of other AdvQueues
	/// </summary>
	/// <typeparam name="T"></typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public class AdvQueue<T> : IEnumerable<T>
	{
		private const double GrowthFactor = 1.5;
		private const int DefaultCapacity = 4;

		private T[] data;
		private int head; // points to the first item to be removed
		private int tail; // points to the place where new items will be added
		private int count;
		private int version;

		public AdvQueue()
		{
			this.data = new T[DefaultCapacity];
		}

		public AdvQueue(int capacity)
		{
			if (capacity < 0)
				throw new ArgumentOutOfRangeException();

			this.data = new T[capacity < DefaultCapacity ? DefaultCapacity : capacity];
		}

		public AdvQueue(IEnumerable<T> source)
		{
			this.data = new T[DefaultCapacity];

			foreach (var item in source)
				Enqueue(item);
		}

		public int Count { get { return count; } }

		public void Enqueue(T item)
		{
			if (count == data.Length) Grow();

			data[tail] = item;
			tail = (tail + 1) % data.Length; // move tail (w/ wrap-around)
			count++;
			version++;

			Debug.Assert(count <= data.Length);
		}

		public void Enqueue(AdvQueue<T> other)
		{
			var otherCount = other.count;
			if (otherCount < 1) return;

			// check if we have enough space
			if (data.Length - count < otherCount) Grow(minimum: count + otherCount);

			// these are easier to read than "other.xxx"
			var otherData = other.data;
			var otherHead = other.head;
			var otherTail = other.tail;

			if (count == 0 && head > 0)
			{
				//we are empty, so reset the pointers
				head = tail = 0;
			}
			else if (head < tail && data.Length - tail < otherCount)
			{
				// head is not at start, but data is continous, and could have
				// enough space without wrapping if we'd move the data back
				// (so empty space after tail becomes large enough)
				Array.Copy(data, head, data, 0, count);
				head = 0;
				tail = count;
			}

			if (otherHead < otherTail)
			{
				// the source queue is continous
				Array.Copy(otherData, otherHead, this.data, this.tail, otherCount);
			}
			else
			{
				// the source queue is wrapping around
				Array.Copy(otherData, otherHead, this.data, tail, otherData.Length - otherHead);
				Array.Copy(otherData, 0, this.data, otherData.Length - otherHead + tail, otherTail);
			}

			tail = (tail + other.count) % data.Length;
			count += other.count;
			other.Clear();

			version++;

			Debug.Assert(count <= data.Length);
		}

		public void Insert(T item)
		{
			// grow the array if needed but shift the items by 1 so that we put
			// the new item at 0
			if (count == data.Length) Grow(shift: 1);

			// move head back by 1
			head = (data.Length + head - 1) % data.Length;
			data[head] = item;
			count++;
			version++;
			Debug.Assert(count <= data.Length);
		}

		public T Peek()
		{
			if (count == 0) throw new InvalidOperationException("Empty queue");

			return this.data[this.head];
		}

		public bool TryPeek(out T retval)
		{
			if (count == 0)
			{
				retval = default(T);

				return false;
			}

			retval = this.data[this.head];

			return true;
		}

		public T Dequeue()
		{
			T retval;

			if (!TryDequeue(out retval))
				throw new InvalidOperationException("Empty queue");

			return retval;
		}

		public bool TryDequeue(out T retval)
		{
			if (count == 0)
			{
				retval = default(T);

				return false;
			}

			retval = data[head];
			data[head] = default(T); // drop reference

			head = (head + 1) % data.Length; // move head (w/ wrap-around)
			count--;
			version++;

			Debug.Assert(count > -1);

			return true;
		}

		public void Clear()
		{
			if (count == 0) return;

			// same as Grow
			if (head < tail)
			{
				Array.Clear(data, head, count);
			}
			else
			{
				Array.Clear(data, head, data.Length - head);
				Array.Clear(data, 0, tail);
			}

			head = tail = count = 0;
			version++;
		}

		private void Grow(int shift = 0, int minimum = 0)
		{
			var capacity = (int)(data.Length * GrowthFactor);
			if (capacity < minimum) capacity = minimum;

			var newData = new T[capacity];

			if (count > 0)
			{
				if (head < tail)
				{
					// the current queue is continous
					// ....****....
					//     ^  ^
					//     H  T
					Array.Copy(data, head, newData, shift, count);
				}
				else
				{
					// the current queue is wrapping around
					// ****....****
					//    ^    ^
					//    T    H
					Array.Copy(data, head, newData, shift, data.Length - head);
					Array.Copy(data, 0, newData, shift + data.Length - head, tail);
				}
			}

			Debug.Assert(capacity > count);
			data = newData;
			head = shift;
			tail = count + shift;
			version++;
		}

		#region [ Enumeration                  ]

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}

		public struct Enumerator : IEnumerator<T>
		{
			private readonly AdvQueue<T> owner;
			private readonly int ownerVersion;
			private int i;
			private T current;

			internal Enumerator(AdvQueue<T> owner)
			{
				this.owner = owner;
				this.ownerVersion = owner.version;
				this.i = 0;
				this.current = default(T);
			}

			public T Current
			{
				get { return current; }
			}

			public void Reset()
			{
				if (ownerVersion != owner.version)
					throw new InvalidOperationException("Queue has changed during enumeration.");

				i = 0;
				current = default(T);
			}

			public bool MoveNext()
			{
				if (ownerVersion == owner.version && i < owner.count)
				{
					current = owner.data[(owner.head + i) % owner.data.Length];
					i++;

					return true;
				}

				if (ownerVersion != owner.version) throw new InvalidOperationException("Queue has changed during enumeration.");

				i = owner.count;
				current = default(T);

				return false;
			}

			object IEnumerator.Current { get { return Current; } }

			public void Dispose() { }
		}

		#endregion
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
