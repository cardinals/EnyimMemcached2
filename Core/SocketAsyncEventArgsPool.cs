using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Enyim.Caching
{
	internal class SocketAsyncEventArgsPool : IDisposable
	{
		internal static readonly SocketAsyncEventArgsPool Instance = new SocketAsyncEventArgsPool(1024 * 1024);

		private readonly int chunkSize;
		private readonly object ExtendLock;

		private ConcurrentStack<Entry> entries;
		private ConcurrentDictionary<SocketAsyncEventArgs, Entry> returnIndex;
		private ConcurrentDictionary<SocketAsyncEventArgs, ArraySegment<byte>> sliceIndex;

		private SocketAsyncEventArgsPool(int chunkSize)
		{
			this.ExtendLock = new object();
			this.chunkSize = chunkSize;
			this.entries = new ConcurrentStack<Entry>();
			this.returnIndex = new ConcurrentDictionary<SocketAsyncEventArgs, Entry>();
			this.sliceIndex = new ConcurrentDictionary<SocketAsyncEventArgs, ArraySegment<byte>>();
		}

		public SocketAsyncEventArgs Take(int size)
		{
			Require.Value("size", size <= chunkSize, "Max size: " + chunkSize);

			ArraySegment<byte> slice;
			Entry last;

			while (!entries.TryPeek(out last) || !last.TryAlloc(size, out slice))
			{
				lock (ExtendLock)
				{
					if (!entries.TryPeek(out last) || !last.TryAlloc(size, out slice))
					{
						entries.Push(new Entry(chunkSize));
					}
				}
			}

			var retval = new SocketAsyncEventArgs();
			var b1 = returnIndex.TryAdd(retval, last);
			var b2 = sliceIndex.TryAdd(retval, slice);

			Debug.Assert(b1);
			Debug.Assert(b2);

			retval.SetBuffer(slice.Array, slice.Offset, slice.Count);

			return retval;
		}

		public void Return(SocketAsyncEventArgs eventArgs)
		{
			var buffer = eventArgs.Buffer;
			Entry entry;
			ArraySegment<byte> slice;

			if (!returnIndex.TryRemove(eventArgs, out entry))
				throw new InvalidOperationException("Unknown buffer, no entry");
			if (!sliceIndex.TryRemove(eventArgs, out slice))
				throw new InvalidOperationException("Unknown buffer, no slice");

			entry.Release(slice);
		}

		public void Compact()
		{
			lock (ExtendLock)
			{
				var all = new Entry[entries.Count];
				var didPop = entries.TryPopRange(all);

				// dispose all empty entries
				for (var i = 0; i < didPop; i++)
				{
					var entry = all[i];
					if (entry == null || !entry.IsEmpty) continue;
#if DEBUG
					foreach (var v in returnIndex.Values)
						if (v == entry)
							throw new InvalidOperationException("Entry is still in returnIndex");

					foreach (var v in sliceIndex.Values)
						if (v.Array == entry.Buffer)
							throw new InvalidOperationException("Entry's buffer is still in sliceIndex");
#endif
					entry.Dispose();
					all[i] = null;
				}

				// put all remainig entries back into the stack
				for (var i = didPop - 1; i >= 0; i--)
				{
					var entry = all[i];
					if (entry != null)
						entries.Push(entry);
				}
			}
		}

		public void Dispose()
		{
			lock (ExtendLock)
			{
				if (entries == null) return;

				Entry entry;

				while (entries.TryPop(out entry))
					entry.Dispose();

				returnIndex.Clear();
				returnIndex = null;
				entries = null;
			}
		}

		#region [ Entry                        ]

		class Entry : IDisposable
		{
			internal byte[] Buffer;
			private GCHandle pin;
			private int offset;
			private int usage;

			public Entry(int size)
			{
				Buffer = new byte[size];
				pin = GCHandle.Alloc(Buffer);
			}

			public void Dispose()
			{
				if (Buffer != null)
				{
					pin.Free();
					Buffer = null;
				}
			}

			public bool TryAlloc(int size, out ArraySegment<byte> retval)
			{
				size = ((size + 7) / 8) * 8;

				while (true)
				{
					var oldOffset = offset;
					var newOffset = oldOffset + size;
					if (newOffset > Buffer.Length)
						break;

					if (Interlocked.CompareExchange(ref offset, newOffset, oldOffset) == oldOffset)
					{
						retval = new ArraySegment<byte>(Buffer, oldOffset, size);
						Interlocked.Add(ref usage, size);
						return true;
					}
				}

				retval = new ArraySegment<byte>();
				return false;
			}

			public void Release(ArraySegment<byte> segment)
			{
				Require.That(segment.Array == Buffer, "Segment is not owned by this entry");

				var newValue = Interlocked.Add(ref usage, -segment.Count);

				Debug.Assert(newValue >= 0, "double-release somewhere");
			}

			public bool IsEmpty
			{
				get { return Interlocked.CompareExchange(ref usage, 0, 0) == 0; }
			}
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
