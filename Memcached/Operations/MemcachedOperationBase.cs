﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Enyim.Caching.Memcached.Operations
{
	public abstract class MemcachedOperationBase<TResult> : IOperation, IHaveResult<TResult>, IMemcachedOperation
	{
		protected MemcachedOperationBase(IBufferAllocator allocator)
		{
			Allocator = allocator;
		}

		protected abstract BinaryRequest CreateRequest();
		protected abstract TResult CreateResult(BinaryResponse response);

		protected IBufferAllocator Allocator { get; private set; }
		public TResult Result { get; private set; }
		protected uint CorrelationId { get; private set; }

		IRequest IOperation.CreateRequest()
		{
			var retval = CreateRequest();
			CorrelationId = retval.CorrelationId;

			return retval;
		}

		bool IOperation.Handles(IResponse response)
		{
			return CorrelationId == ((BinaryResponse)response).CorrelationId;
		}

		bool IOperation.ProcessResponse(IResponse response)
		{
			var result = CreateResult((BinaryResponse)response);
			Result = result;

			return result == null;
		}
	}

	public interface IMemcachedOperation
	{
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
