using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public abstract partial class MemcachedClientBase
	{
		private readonly ICluster cluster;
		private readonly IOperationFactory opFactory;
		private readonly ITranscoder transcoder;
		private readonly IKeyTransformer keyTransformer;

		protected MemcachedClientBase(IContainer container)
			: this(container.Resolve<ICluster>(),
					container.Resolve<IOperationFactory>(),
					container.Resolve<IKeyTransformer>(),
					container.Resolve<ITranscoder>())
		{ }

		protected MemcachedClientBase(ICluster cluster, IOperationFactory opFactory, IKeyTransformer keyTransformer, ITranscoder transcoder)
		{
			this.cluster = cluster;
			this.opFactory = opFactory;
			this.keyTransformer = keyTransformer;
			this.transcoder = transcoder;
		}

		#region [ Expiration helper            ]

		protected const int MaxSeconds = 60 * 60 * 24 * 30;
		protected static readonly DateTime UnixEpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		protected static uint GetExpiration(TimeSpan validFor)
		{
			// infinity
			if (validFor == TimeSpan.Zero || validFor == TimeSpan.MaxValue) return 0;

			var seconds = (uint)validFor.TotalSeconds;
			if (seconds < MaxSeconds) return seconds;

			return GetExpiration(SystemTime.Now() + validFor);
		}

		protected static uint GetExpiration(DateTime expiresAt)
		{
			if (expiresAt == DateTime.MaxValue || expiresAt == DateTime.MinValue) return 0;
			if (expiresAt < UnixEpochUtc) throw new ArgumentOutOfRangeException("expiresAt must be > " + UnixEpochUtc);

			return (uint)(expiresAt.ToUniversalTime() - UnixEpochUtc).TotalSeconds;
		}

		#endregion

		protected virtual async Task<IGetOperationResult> PerformGetCore(string key)
		{
			var op = opFactory.Get(keyTransformer.Transform(key));
			await cluster.Execute(op);

			return op.Result;
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
	}
}
