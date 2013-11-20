using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached.Results;
using Funq;

namespace Enyim.Caching.Memcached
{
	public partial class MemcachedClient
	{
		protected async Task<Dictionary<string, IGetOperation>> MultiGetCore(IEnumerable<string> keys)
		{
			var ops = new Dictionary<string, IGetOperation>();
			var tasks = new List<Task>();

			foreach (var key in keys)
			{
				var op = opFactory.Get(keyTransformer.Transform(key));
				tasks.Add(cluster.Execute(op));
				ops[key] = op;
			}

			await Task.WhenAll(tasks);

			return ops;
		}

		protected virtual async Task<IGetOperationResult> PerformGetCore(string key)
		{
			var op = opFactory.Get(keyTransformer.Transform(key));
			await cluster.Execute(op);

			return op.Result;
		}

		protected IGetOperationResult<T> ConvertToResult<T>(IGetOperationResult result)
		{
			var retval = result.Combine(new GetOperationResult<T>());

			if (retval.Success)
			{
				var value = transcoder.Deserialize(result.Value);
				retval.Value = (T)value;
			}

			return retval;
		}

		protected object ConvertToValue(IGetOperationResult result)
		{
			if (result.Success)
			{
				var value = transcoder.Deserialize(result.Value);
				return value;
			}

			return null;
		}

		protected async Task<IOperationResult> PerformStoreAsync(StoreMode mode, string key, object value, uint expires, ulong cas)
		{
			var ci = transcoder.Serialize(value);
			var op = opFactory.Store(mode, keyTransformer.Transform(key), ci, cas, expires);
			await cluster.Execute(op);

			return op.Result;
		}

		protected async Task<IOperationResult> PerformRemove(string key, ulong cas)
		{
			var op = opFactory.Delete(keyTransformer.Transform(key), cas);
			await cluster.Execute(op);

			return op.Result;
		}

		protected async Task<IOperationResult> PerformConcate(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
		{
			var op = opFactory.Concat(mode, keyTransformer.Transform(key), cas, data);
			await cluster.Execute(op);

			return op.Result;
		}

		protected async Task<IMutateOperationResult> PerformMutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			var op = opFactory.Mutate(mode, keyTransformer.Transform(key), defaultValue, delta, cas, expires);
			await cluster.Execute(op);

			return op.Result;
		}

	}
}
