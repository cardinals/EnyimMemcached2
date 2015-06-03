using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching.Memcached.Operations;

namespace Enyim.Caching.Memcached.Results
{
	public static class OperationResultExtensions
	{
		public static T Success<T>(this T self, BinarySingleItemOperation<T> op)
			where T : class, IOperationResult
		{
			self.Success = true;
			self.Cas = op.Cas;

			return self;
		}

		public static T Failed<T>(this T self, BinarySingleItemOperation<T> op, Exception exception)
			where T : class, IOperationResult
		{
			self.Success = false;
			self.Cas = op.Cas;
			self.Message = exception.Message;
			self.Exception = exception;

			return self;
		}

		public static T FailWith<T>(this T self, Exception exception = null)
			where T : class, IOperationResult
		{
			self.Success = false;
			self.StatusCode = (int)StatusCode.InternalError;

			if (exception != null)
			{
				self.Message = exception.Message;
				self.Exception = exception;
			}

			return self;
		}

		public static T NotFound<T>(this T self, BinarySingleItemOperation<T> op)
			where T : class, IOperationResult
		{
			self.Success = false;
			self.Cas = op.Cas;
			self.Message = "NOT_FOUND";
			self.Exception = new KeyNotFoundException(Encoding.UTF8.GetString(op.Key.Array, 0, op.Key.Length));
			self.StatusCode = 1;

			return self;
		}

		public static T WithResponse<T>(this T self, BinaryResponse response, string failMessage = null)
			where T : IOperationResult
		{
			var success = response.StatusCode == 0;

			self.StatusCode = response.StatusCode;
			self.Success = success;
			self.Message = success ? null : response.GetStatusMessage() ?? failMessage;
			self.Cas = response.CAS;

			return self;
		}

		/// <summary>
		/// Combine will attempt to minimize the depth of InnerResults and maintain status codes
		/// </summary>
		/// <param name="target"></param>
		public static T UpdateFrom<T>(this T target, IOperationResult source)
			where T : IOperationResult
		{
			target.Cas = source.Cas;
			target.Message = source.Message;
			target.Success = source.Success;
			target.Exception = source.Exception;
			target.StatusCode = source.StatusCode ?? target.StatusCode;
			target.InnerResult = source.InnerResult ?? source;

			return target;
		}

		/// <summary>
		/// Combine will attempt to minimize the depth of InnerResults and maintain status codes
		/// </summary>
		/// <param name="target"></param>
		public static T TryFailFrom<T>(this T target, IOperationResult source)
			where T : IOperationResult
		{
			if (!source.Success)
			{
				target.Cas = source.Cas;
				target.Message = source.Message;
				target.Success = source.Success;
				target.Exception = source.Exception;
				target.StatusCode = source.StatusCode ?? target.StatusCode;
				target.InnerResult = source.InnerResult ?? source;
			}

			return target;
		}
	}
}

#region [ License information          ]

/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2012 Attila Kiskó, enyim.com
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
