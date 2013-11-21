using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enyim.Caching.Integrations.Funq;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public abstract partial class MemcachedClientBase
	{
		public const string ClusterSection = "enyim.com/memcached/cluster";
		public const string ClientSection = "enyim.com/memcached/client";

		protected readonly ICluster Cluster;
		protected readonly IOperationFactory OpFactory;
		protected readonly ITranscoder Transcoder;
		protected readonly IKeyTransformer KeyTransformer;

		protected MemcachedClientBase()
			: this(DefaultContainer) { }

		protected MemcachedClientBase(IContainer container)
			: this
			(
				container.Resolve<ICluster>(),
				container.Resolve<IOperationFactory>(),
				container.Resolve<IKeyTransformer>(),
				container.Resolve<ITranscoder>()
			) { }

		protected MemcachedClientBase(ICluster cluster, IOperationFactory opFactory, IKeyTransformer keyTransformer, ITranscoder transcoder)
		{
			this.Cluster = cluster;
			this.OpFactory = opFactory;
			this.KeyTransformer = keyTransformer;
			this.Transcoder = transcoder;
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
			if (expiresAt <= UnixEpochUtc) throw new ArgumentOutOfRangeException("expiresAt must be > " + UnixEpochUtc);

			return (uint)(expiresAt.ToUniversalTime() - UnixEpochUtc).TotalSeconds;
		}

		#endregion

#if !STANDALONE
		private static readonly Funq.Container rootContainer;
		private static readonly IContainer DefaultContainer;

		static MemcachedClientBase()
		{
			rootContainer = new Funq.Container();
			DefaultContainer = new ContainerWrapper(rootContainer);

			rootContainer.AddDefaultServices();
			rootContainer.RegisterClusterFromConfig(ClusterSection);
		}

		public static IContainer WithCluster(string sectionName)
		{
			var container = rootContainer.CreateChildContainer();
			container.RegisterClusterFromConfig(sectionName);

			return new ContainerWrapper(container);
		}
#endif

		protected virtual async Task<IGetOperationResult> PerformGetCore(string key)
		{
			var op = OpFactory.Get(KeyTransformer.Transform(key));
			await Cluster.Execute(op);

			return op.Result;
		}

		protected async Task<IOperationResult> PerformStoreAsync(StoreMode mode, string key, object value, uint expires, ulong cas)
		{
			var ci = Transcoder.Serialize(value);
			var op = OpFactory.Store(mode, KeyTransformer.Transform(key), ci, cas, expires);
			await Cluster.Execute(op);

			return op.Result;
		}

		protected async Task<IOperationResult> PerformRemove(string key, ulong cas)
		{
			var op = OpFactory.Delete(KeyTransformer.Transform(key), cas);
			await Cluster.Execute(op);

			return op.Result;
		}

		protected async Task<IOperationResult> PerformConcate(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
		{
			var op = OpFactory.Concat(mode, KeyTransformer.Transform(key), cas, data);
			await Cluster.Execute(op);

			return op.Result;
		}

		protected async Task<IMutateOperationResult> PerformMutate(MutationMode mode, string key, ulong defaultValue, ulong delta, ulong cas, uint expires)
		{
			var op = OpFactory.Mutate(mode, KeyTransformer.Transform(key), defaultValue, delta, cas, expires);
			await Cluster.Execute(op);

			return op.Result;
		}

		protected async Task<Dictionary<string, IGetOperation>> MultiGetCore(IEnumerable<string> keys)
		{
			var ops = new Dictionary<string, IGetOperation>();
			var tasks = new List<Task>();

			foreach (var key in keys)
			{
				var op = OpFactory.Get(KeyTransformer.Transform(key));
				tasks.Add(Cluster.Execute(op));
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
				var value = Transcoder.Deserialize(result.Value);
				retval.Value = (T)value;
			}

			return retval;
		}

		protected object ConvertToValue(IGetOperationResult result)
		{
			if (result.Success)
			{
				var value = Transcoder.Deserialize(result.Value);
				return value;
			}

			return null;
		}
	}
}
