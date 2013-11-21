using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Enyim.Caching.Memcached;
using System.Threading;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching.Memcached
{
	public class MemcachedNode : NodeBase
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private bool didNoop;

		public MemcachedNode(IPEndPoint endpoint, IFailurePolicy failurePolicy, ISocket socket)
			: base(endpoint, failurePolicy, socket) { }

		public override bool Send()
		{
			didNoop = false;

			return base.Send();
		}

		protected override IResponse CreateResponse()
		{
			return new BinaryResponse();
		}

		protected override void BeforeWriteOp(SegmentListCopier copier, WriteBuffer writeBuffer, IOperation op)
		{
			// TODO handle manual NoOp
			if (copier.Length > writeBuffer.Length - writeBuffer.Position - NoOp.BufferSize)
			{
				IntroduceNoOp();
				didNoop = true;
			}
		}

		protected override void FinalizeWriteBuffer(WriteBuffer writeBuffer)
		{
			if (!didNoop)
				IntroduceNoOp();
		}

		/// <summary>
		/// Terminates the write buffer with a noop to force a response after a series of Quiet ops.
		/// </summary>
		private void IntroduceNoOp()
		{
			var noop = new NoOp();
			AddToBuffer(noop);

			if (log.IsTraceEnabled)
				log.Trace("Adding Noop to the write buffer");
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
