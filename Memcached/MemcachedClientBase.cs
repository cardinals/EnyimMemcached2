﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	public abstract partial class MemcachedClientBase
	{
		public static IContainer DefaultContainer;

		private readonly ICluster cluster;
		private readonly IOperationFactory opFactory;
		private readonly ITranscoder transcoder;
		private readonly IKeyTransformer keyTransformer;

		protected MemcachedClientBase() : this(GetOrThrowContainer()) { }

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

		private static IContainer GetOrThrowContainer()
		{
			var retval = DefaultContainer;
			if (retval == null)
				throw new InvalidOperationException("The DefaultContainer must be set before using the default constructor");

			return retval;
		}

		protected virtual async Task<IGetOperationResult> PerformGetCore(string key, ulong cas)
		{
			try
			{
				using (var kb = keyTransformer.Transform(key))
				{
					var op = opFactory.Get(kb, cas);
					await cluster.Execute(op).ConfigureAwait(false);

					return op.Result;
				}
			}
			catch (IOException e)
			{
				return new GetOperationResult().FailWith(e);
			}
		}

		protected virtual async Task<IGetOperationResult> PerformGetAndTouchCore(string key, Expiration expiration, ulong cas)
		{
			try
			{
				using (var kb = keyTransformer.Transform(key))
				{
					var op = opFactory.GetAndTouch(kb, expiration.Value, cas);
					await cluster.Execute(op).ConfigureAwait(false);

					return op.Result;
				}
			}
			catch (IOException e)
			{
				return new GetOperationResult().FailWith(e);
			}
		}

		protected async Task<IOperationResult> PerformStoreAsync(StoreMode mode, string key, object value, Expiration expiration, ulong cas)
		{
			try
			{
				using (var ci = transcoder.Serialize(value))
				using (var kb = keyTransformer.Transform(key))
				{
					var op = opFactory.Store(mode, kb, ci, expiration.Value, cas);
					await cluster.Execute(op).ConfigureAwait(false);

					return op.Result;
				}
			}
			catch (IOException e)
			{
				return new BinaryOperationResult().FailWith(e);
			}
		}

		protected async Task<IOperationResult> PerformRemove(string key, ulong cas)
		{
			try
			{
				using (var kb = keyTransformer.Transform(key))
				{
					var op = opFactory.Delete(kb, cas);
					await cluster.Execute(op).ConfigureAwait(false);

					return op.Result;
				}
			}
			catch (IOException e)
			{
				return new BinaryOperationResult().FailWith(e);
			}
		}

		protected async Task<IOperationResult> PerformConcate(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
		{
			try
			{
				using (var kb = keyTransformer.Transform(key))
				{
					var op = opFactory.Concat(mode, kb, data, cas);
					await cluster.Execute(op).ConfigureAwait(false);

					return op.Result;
				}
			}
			catch (IOException e)
			{
				return new BinaryOperationResult().FailWith(e);
			}
		}

		protected async Task<IMutateOperationResult> PerformMutate(MutationMode mode, string key, Expiration expiration, ulong delta, ulong defaultValue, ulong cas)
		{
			try
			{
				using (var kb = keyTransformer.Transform(key))
				{
					var op = opFactory.Mutate(mode, kb, expiration.Value, delta, defaultValue, cas);
					await cluster.Execute(op).ConfigureAwait(false);

					return op.Result;
				}
			}
			catch (IOException e)
			{
				return new MutateOperationResult().FailWith(e);
			}
		}

		protected async Task<Dictionary<string, IGetOperation>> MultiGetCore(IEnumerable<string> keys)
		{
			var ops = new Dictionary<string, IGetOperation>();
			var tasks = new List<Task>();
			var known = new HashSet<string>();

			using (var ad = new AggregateDisposable())
			{
				foreach (var key in keys)
				{
					if (!known.Add(key)) continue;

					try
					{
						var op = opFactory.Get(ad.Add(keyTransformer.Transform(key)), Protocol.NO_CAS);
						tasks.Add(cluster.Execute(op));

						ops[key] = op;
					}
					catch (IOException e)
					{
						tasks.Add(Task.FromResult(new GetOperationResult().FailWith(e)));
					}
				}

				await Task.WhenAll(tasks).ConfigureAwait(false);
			}

			return ops;
		}

		class AggregateDisposable : IDisposable
		{
			private List<IDisposable> ds = new List<IDisposable>();

			public void Add(IDisposable d)
			{
				ds.Add(d);
			}

			public T Add<T>(T d)
				where T : IDisposable
			{
				ds.Add(d);

				return d;
			}

			public void Dispose()
			{
				if (ds != null)
				{
					foreach (var d in ds)
						d.Dispose();

					ds = null;
				}
			}
		}

		protected async Task<Dictionary<string, IGetOperation>> MultiGetCore(IEnumerable<KeyValuePair<string, ulong>> items)
		{
			var ops = new Dictionary<string, IGetOperation>();
			var tasks = new List<Task>();
			var known = new HashSet<string>();

			using (var ad = new AggregateDisposable())
			{
				foreach (var item in items)
				{
					var key = item.Key;
					if (!known.Add(key)) continue;

					try
					{
						var op = opFactory.Get(ad.Add(keyTransformer.Transform(key)), item.Value);
						tasks.Add(cluster.Execute(op));

						ops[key] = op;
					}
					catch (IOException e)
					{
						tasks.Add(Task.FromResult(new GetOperationResult().FailWith(e)));
					}
				}

				await Task.WhenAll(tasks).ConfigureAwait(false);
			}

			return ops;
		}

		protected async Task<IOperationResult> PerformFlushAll()
		{
			var parts = await cluster.Broadcast(n => opFactory.Flush()).ConfigureAwait(false);

			return new BinaryOperationResult { Success = true };
		}

		protected async Task<IStatsOperationResult> PerformStats(string key)
		{
			var ops = new List<Tuple<IStatsOperation, INode>>();
			// TODO catch this?
			await cluster.Broadcast(n =>
			{
				var op = opFactory.Stats(key);
				ops.Add(Tuple.Create(op, n));

				return op;
			}).ConfigureAwait(false);

			var stats = new ServerStats();
			StatsOperationResult retval = null;

			foreach (var pair in ops)
			{
				var nodeResult = pair.Item1.Result;

				if (retval == null)
					retval = new StatsOperationResult { Value = stats }.UpdateFrom(nodeResult);
				else
					retval.TryFailFrom(nodeResult);

				var nodeStats = nodeResult.Value;
				if (nodeStats == null || nodeStats.Count == 0)
					continue;

				stats.Append(pair.Item2.EndPoint, nodeStats);
			}

			return retval ?? new StatsOperationResult { Value = new ServerStats() }.FailWith();
		}

		protected async Task<IOperationResult> PerformTouch(string key, Expiration expiration, ulong cas)
		{
			try
			{
				using (var kb = keyTransformer.Transform(key))
				{
					var op = opFactory.Touch(kb, expiration.Value, cas);
					await cluster.Execute(op).ConfigureAwait(false);

					return op.Result;
				}
			}
			catch (IOException e)
			{
				return new BinaryOperationResult().FailWith(e);
			}
		}

		#region [ Value helpers                ]

		protected IGetOperationResult<T> ConvertToResult<T>(IGetOperationResult result)
		{
			var retval = new GetOperationResult<T>().UpdateFrom(result);

			if (retval.Success)
			{
				try { retval.Value = (T)transcoder.Deserialize(result.Value); }
				catch (Exception e)
				{
					LogTo.Error(e, "Failed to convert result to " + typeof(T));

					retval.Value = default(T);
					retval.FailWith(e);
				}
			}

			return retval;
		}

		protected object ConvertToValue(IGetOperationResult result)
		{
			if (result.Success)
			{
				try { return transcoder.Deserialize(result.Value); }
				catch (Exception e)
				{
					LogTo.Error(e, "Failed to deserialize value.");
				}
			}

			return null;
		}

		#endregion
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
