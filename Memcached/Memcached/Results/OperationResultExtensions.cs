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
			where T : IOperationResult
		{
			self.Success = true;
			self.Cas = op.Cas;

			return self;
		}

		public static T Fail<T>(this T self, BinarySingleItemOperation<T> op, Exception exception)
			where T : IOperationResult
		{
			self.Success = false;
			self.Cas = op.Cas;
			self.Message = exception.Message;
			self.Exception = exception;

			return self;
		}

		public static T NotFound<T>(this T self, BinarySingleItemOperation<T> op)
			where T : IOperationResult
		{
			self.Success = false;
			self.Cas = op.Cas;
			self.Message = "NOT_FOUND";
			self.Exception = new KeyNotFoundException(op.Key);
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
	}

	public enum StatusCode
	{
		NoError = 0x0000,
		KeyNotFound = 0x0001,
		KeyExists = 0x0002,
		ValueTooLarge = 0x0003,
		InvalidArguments = 0x0004,
		ItemNotStored = 0x0005,
		IncrDecrNonNumericValue = 0x0006,
		NotMyVBucket = 0x0007,
		AuthenticationError = 0x0008,
		AuthenticationContinue = 0x0009,
		UnknownCommand = 0x0081,
		OutOfMemory = 0x0082,
		NotSupported = 0x0083,
		InternalError = 0x0084,
		Busy = 0x0085,
		TemporaryFailure = 0x0086
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