using System;
using Enyim.Caching.Integrations.Funq;
using Funq;

namespace Enyim.Caching.Memcached
{
	public partial class MemcachedClient : IMemcachedClient, IMemcachedClientWithResults, IDisposable
	{
		public const string ClusterSection = "enyim.com/memcached/cluster";
		public const string ClientSection = "enyim.com/memcached/client";

		private ICluster cluster;
		private bool owns;
		private IOperationFactory opFactory;
		private ITranscoder transcoder;
		private IKeyTransformer keyTransformer;

		public MemcachedClient()
			: this(DefaultContainer) { }

		public MemcachedClient(IContainer container)
			: this
			(
				container.Resolve<ICluster>(),
				container.Resolve<IOperationFactory>(),
				container.Resolve<IKeyTransformer>(),
				container.Resolve<ITranscoder>()
			) { }

		public MemcachedClient(ICluster cluster, IOperationFactory opFactory, IKeyTransformer keyTransformer, ITranscoder transcoder)
		{
			this.cluster = cluster;
			this.opFactory = opFactory;
			this.keyTransformer = keyTransformer;
			this.transcoder = transcoder;
		}

		~MemcachedClient()
		{
			Dispose();
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);

			if (owns && cluster != null)
			{
				cluster.Dispose();
				cluster = null;
			}
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
		public static readonly IContainer DefaultContainer;

		static MemcachedClient()
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
	}
}
