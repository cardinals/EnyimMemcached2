using System;
using System.Collections.Generic;
using System.Linq;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Tests
{
	public class _FailurePolicy : IFailurePolicy
	{
		internal _FailurePolicy(StepRecorder recorder)
		{
			recorder.Mark("FailurePolicy");
		}

		public int Property { get; set; }
		public int Counter { get; set; }

		public void Reset(INode node)
		{
		}

		public bool ShouldFail(INode node)
		{
			return false;
		}
	}

	public class _NodeLocator : INodeLocator
	{
		internal _NodeLocator(StepRecorder recorder)
		{
			recorder.Mark("NodeLocator");
		}

		public string Property { get; set; }

		public void Initialize(IEnumerable<INode> nodes)
		{
		}

		public INode Locate(byte[] key)
		{
			return null;
		}
	}

	public class _ReconnectPolicy : IReconnectPolicy
	{
		internal _ReconnectPolicy(StepRecorder recorder)
		{
			recorder.Mark("ReconnectPolicy");
		}

		public TimeSpan Property { get; set; }

		public TimeSpan Schedule(INode node)
		{
			return TimeSpan.Zero;
		}

		public void Reset(INode node)
		{
		}
	}

	public class _KeyTransformer : IKeyTransformer
	{
		public byte[] Transform(string key)
		{
			throw new NotImplementedException();
		}
	}

	public class _Transcoder : ITranscoder
	{
		public CacheItem Serialize(object value)
		{
			throw new NotImplementedException();
		}

		public object Deserialize(CacheItem item)
		{
			throw new NotImplementedException();
		}
	}

	public class _Operationfactory : IOperationFactory
	{
		public IGetOperation Get(string key)
		{
			throw new NotImplementedException();
		}

		public IStoreOperation Store(StoreMode mode, string key, CacheItem value, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IDeleteOperation Delete(string key, ulong cas)
		{
			throw new NotImplementedException();
		}

		public IMutateOperation Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IConcatOperation Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
		{
			throw new NotImplementedException();
		}

		public IFlushOperation Flush()
		{
			throw new NotImplementedException();
		}

		public IGetOperation Get(byte[] key)
		{
			throw new NotImplementedException();
		}

		public IStoreOperation Store(StoreMode mode, byte[] key, CacheItem value, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IDeleteOperation Delete(byte[] key, ulong cas)
		{
			throw new NotImplementedException();
		}

		public IMutateOperation Mutate(MutationMode mode, byte[] key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			throw new NotImplementedException();
		}

		public IConcatOperation Concat(ConcatenationMode mode, byte[] key, ulong cas, ArraySegment<byte> data)
		{
			throw new NotImplementedException();
		}

		public IStatsOperation Stats(string type)
		{
			throw new NotImplementedException();
		}

		public IGetAndTouchOperation GetAndTouch(byte[] key, uint expires)
		{
			throw new NotImplementedException();
		}

		public ITouchOperation Touch(byte[] key, uint expires)
		{
			throw new NotImplementedException();
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
