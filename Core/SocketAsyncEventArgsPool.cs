using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace Enyim.Caching
{
	/// <summary>
	/// Provides an efficient way of creating SocketAsyncEventArgs that are using pinned buffers.
	/// It assumes that the returned SocketAsyncEventArgs are long-lived and not allocated frequently.
	/// (Which is true for the AsyncSocket/ClusterBase.)
	/// </summary>
	internal class SocketAsyncEventArgsFactory : IDisposable
	{
		internal static readonly SocketAsyncEventArgsFactory Instance = new SocketAsyncEventArgsFactory(1 * 1024 * 1024);

		private readonly object UpdateLock;
		private readonly int chunkSize;

		// list of factories; the empty one is always at the top
		private ConcurrentStack<BufferFactory> factories;
		// tracks which SAEA's inner buffer belongs to which factory
		private ConcurrentDictionary<SocketAsyncEventArgs, Tuple<BufferFactory, ArraySegment<byte>>> knownEventArgs;

		private SocketAsyncEventArgsFactory(int chunkSize)
		{
			this.UpdateLock = new object();
			this.chunkSize = chunkSize;

			this.factories = new ConcurrentStack<BufferFactory>();
			this.knownEventArgs = new ConcurrentDictionary<SocketAsyncEventArgs, Tuple<BufferFactory, ArraySegment<byte>>>();
		}

		internal SocketAsyncEventArgs Take(int size)
		{
			if (size > chunkSize)
				throw new ArgumentOutOfRangeException(String.Format("Required buffer {0} size is larger than the chunk size {1}", size, chunkSize));

			BufferFactory bufferFactory;
			ArraySegment<byte> segment;
			bool success;

			// allocate new pools until we can acquire a buffer with the required size
			// The way SAEAs and buffers are used we're fine with continously allocating
			// new BufferFactories when we run out of space (and not being able to reuse them)
			while (!factories.TryPeek(out bufferFactory) || !bufferFactory.TryAlloc(size, out segment))
			{
				lock (UpdateLock)
				{
					if (!factories.TryPeek(out bufferFactory) || !bufferFactory.TryAlloc(size, out segment))
					{
						// create & store a new factory, allocation will be handled by the while loop
						factories.Push(new BufferFactory(chunkSize));
					}
				}
			}

			var retval = new SocketAsyncEventArgs();
			retval.SetBuffer(segment.Array, segment.Offset, segment.Count);

			success = knownEventArgs.TryAdd(retval, Tuple.Create(bufferFactory, segment));
			Debug.Assert(success);

			return retval;
		}

		internal void Return(SocketAsyncEventArgs eventArgs)
		{
			Tuple<BufferFactory, ArraySegment<byte>> entry;

			if (!knownEventArgs.TryRemove(eventArgs, out entry))
				throw new InvalidOperationException("Unknown SocketAsyncEventArgs, could not map it to a BufferFactory.");

			entry.Item1.Forget(entry.Item2);
		}

		/// <summary>
		/// Compacts the buffer list by removing and destroying all empty BufferFactories.
		/// </summary>
		internal void Compact()
		{
			lock (UpdateLock)
			{
				if (factories.Count == 0) return;

				var all = new BufferFactory[factories.Count];
				var didPop = factories.TryPopRange(all);

				// dispose all empty entries
				// and put the rest back into the stack
				for (var i = didPop - 1; i >= 0; i--)
				{
					var current = all[i];
					Debug.Assert(current != null);

					if (current.IsEmpty)
					{
						#region debug checks
#if DEBUG
						foreach (var tmp in knownEventArgs.Values)
						{
							if (tmp.Item1 == current)
								throw new InvalidOperationException("Factory is still mapped to a SocketAsyncEventArgs");
						}
#endif
						#endregion

						current.Dispose();
					}
					else
					{
						factories.Push(current);
					}
				}
			}
		}

		public void Dispose()
		{
			lock (UpdateLock)
			{
				if (factories == null) return;

				BufferFactory factory;

				foreach (var tmp in knownEventArgs.Keys)
					try { tmp.Dispose(); }
					catch { }

				while (factories.TryPop(out factory))
					try { factory.Dispose(); }
					catch { }

				knownEventArgs.Clear();
				knownEventArgs = null;
				factories = null;
			}
		}

		#region [ BufferFactory                ]

		/// <summary>
		/// Implements a buffer factory. Returns ArraySegments using a pinned byte array as underlying storage.
		/// </summary>
		/// <remarks>
		/// The BufferFactory only cares if all buffers created by it are returned without being able to reuse them.
		/// (SAEAFactory will clean up the empty BufferFactories during compaction.)
		/// </remarks>
		private class BufferFactory : IDisposable
		{
			private byte[] data;
			private GCHandle pin;
			private int offset;
			private int usage;

			public BufferFactory(int size)
			{
				data = new byte[size];
				pin = GCHandle.Alloc(data);
			}

			public void Dispose()
			{
				if (data != null)
				{
					pin.Free();
					data = null;
				}
			}

			public bool IsEmpty
			{
				get { return Interlocked.CompareExchange(ref usage, 0, 0) == 0; }
			}

			public bool TryAlloc(int size, out ArraySegment<byte> buffer)
			{
				// round up to multiplies of 8
				size = ((size + 7) / 8) * 8;

				// repeat until we manage to allocate the buffer or we run out of space
				while (true)
				{
					var oldOffset = offset;
					var newOffset = oldOffset + size;
					if (newOffset > data.Length)
						break;

					if (Interlocked.CompareExchange(ref offset, newOffset, oldOffset) == oldOffset)
					{
						buffer = new ArraySegment<byte>(data, oldOffset, size);
						Interlocked.Add(ref usage, size);
						return true;
					}
				}

				// cannot allocate the required amount
				buffer = new ArraySegment<byte>();
				return false;
			}

			public void Forget(ArraySegment<byte> segment)
			{
				Debug.Assert(segment.Array == data, "Segment is not owned by this pool");

				var newValue = Interlocked.Add(ref usage, -segment.Count);

				Debug.Assert(newValue >= 0, "a segment was released twice");
			}

#if DEBUG
			public bool IsOwned(ArraySegment<byte> buffer)
			{
				return buffer.Array == data;
			}
#endif
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
