using System;

namespace Enyim.Caching.Memcached.Operations
{
	public abstract class BinaryOperation : IOperation
	{
		public int StatusCode { get; protected set; }

		private uint correlationId;

		public IRequest GetRequest()
		{
			var retval = DoGetRequest();
			correlationId = retval.CorrelationId;

			return retval;
		}

		public void ProcessResponse(IResponse response)
		{
			//StatusCode = response == null ? 0 : response.StatusCode;
			DoProcessResponse((BinaryResponse)response);
		}

		public bool Matches(IResponse response)
		{
			return correlationId == ((BinaryResponse)response).CorrelationId;
		}

		protected abstract BinaryRequest DoGetRequest();
		protected abstract void DoProcessResponse(BinaryResponse response);
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
