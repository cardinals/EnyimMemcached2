using System;

namespace Enyim.Caching.Memcached
{
	public partial class MemcachedClient : IMemcachedClient, IMemcachedClientWithResults, IDisposable
	{
		private ICluster cluster;
		private bool owns;
		private IMemcachedClientConfiguration configuration;
		private IOperationFactory opFactory;
		private ITranscoder transcoder;
		private IKeyTransformer keyTransformer;

		protected MemcachedClient() { }

		public MemcachedClient(IMemcachedClientConfiguration configuration, ICluster cluster) : this(configuration, cluster, false) { }

		protected MemcachedClient(IMemcachedClientConfiguration configuration, ICluster cluster, bool owns)
		{
			this.cluster = cluster;
			this.configuration = configuration;
			this.owns = owns;

			this.opFactory = configuration.OperationFactory;
			this.keyTransformer = configuration.KeyTransformer;
			this.transcoder = configuration.Transcoder;
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
	}
}
