using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public partial class MemcachedClient : MemcachedClientBase, IMemcachedClient
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();

		public MemcachedClient() : base() { }
		public MemcachedClient(IContainer container) : base(container) { }
		public MemcachedClient(ICluster cluster, IOperationFactory opFactory, IKeyTransformer keyTransformer, ITranscoder transcoder)
			: base(cluster, opFactory, keyTransformer, transcoder)
		{ }

		public Task<T> GetAsync<T>(string key)
		{
			return DoGet<T>(PerformGetCore(key, 0));
		}

		public async Task<IDictionary<string, object>> GetAsync(IEnumerable<string> keys)
		{
			try
			{
				var ops = await MultiGetCore(keys).ConfigureAwait(false);
				var retval = new Dictionary<string, object>();

				foreach (var kvp in ops)
				{
					try
					{
						retval[kvp.Key] = ConvertToValue(kvp.Value.Result);
					}
					catch (Exception e)
					{
						if (log.IsErrorEnabled) log.Error(e);
						retval[kvp.Key] = null;
					}
				}

				return retval;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled) log.Error(e);

				return new Dictionary<string, object>();
			}
		}

		public Task<T> GetAndTouchAsync<T>(string key, DateTime expiresAt)
		{
			return DoGet<T>(PerformGetAndTouchCore(key, GetExpiration(expiresAt)));
		}

		public Task<T> GetAndTouchAsync<T>(string key, TimeSpan validFor)
		{
			return DoGet<T>(PerformGetAndTouchCore(key, GetExpiration(validFor)));
		}

		private async Task<T> DoGet<T>(Task<Results.IGetOperationResult> getter)
		{
			try
			{
				var result = await getter.ConfigureAwait(false);
				var converted = ConvertToValue(result);

				return (T)converted;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled) log.Error(e);

				return default(T);
			}
		}

		public Task<bool> TouchAsync(string key, DateTime expiration)
		{
			return HandleErrors(PerformTouch(key, GetExpiration(expiration), Protocol.NO_CAS));
		}

		public Task<bool> TouchAsync(string key, TimeSpan validFor)
		{
			return HandleErrors(PerformTouch(key, GetExpiration(validFor), Protocol.NO_CAS));
		}

		public Task<bool> StoreAsync(StoreMode mode, string key, object value, DateTime expiresAt)
		{
			return HandleErrors(PerformStoreAsync(mode, key, value, GetExpiration(expiresAt), Protocol.NO_CAS));
		}

		public Task<bool> StoreAsync(StoreMode mode, string key, object value, TimeSpan validFor)
		{
			return HandleErrors(PerformStoreAsync(mode, key, value, GetExpiration(validFor), Protocol.NO_CAS));
		}

		public Task<bool> RemoveAsync(string key)
		{
			return HandleErrors(PerformRemove(key, 0));
		}

		public Task<bool> ConcateAsync(ConcatenationMode mode, string key, ArraySegment<byte> data)
		{
			return HandleErrors(PerformConcate(mode, key, 0, data));
		}

		public async Task<ulong> MutateAsync(MutationMode mode, string key, DateTime expiresAt, ulong defaultValue, ulong delta)
		{
			try
			{
				var result = await PerformMutate(mode, key, defaultValue, delta, 0, GetExpiration(expiresAt)).ConfigureAwait(false);

				return result.Value;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled) log.Error(e);

				return 0;
			}
		}

		public async Task<ulong> MutateAsync(MutationMode mode, string key, TimeSpan validFor, ulong defaultValue, ulong delta)
		{
			try
			{
				var result = await PerformMutate(mode, key, defaultValue, delta, 0, GetExpiration(validFor)).ConfigureAwait(false);

				return result.Value;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled) log.Error(e);

				return 0;
			}
		}

		public async Task<ServerStats> StatsAsync(string key)
		{
			try
			{
				var result = await PerformStats(key).ConfigureAwait(false);

				return result.Value;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled) log.Error(e);

				return ServerStats.Empty;
			}
		}

		public Task<bool> FlushAllAsync()
		{
			return HandleErrors(PerformFlushAll());
		}

		private static async Task<bool> HandleErrors(Task<Results.IOperationResult> task)
		{
			try
			{
				var result = await task.ConfigureAwait(false);

				return result.Success;
			}
			catch (Exception e)
			{
				if (log.IsErrorEnabled) log.Error(e);

				return false;
			}
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
