using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Enyim.Caching
{
	public static class Metrics
	{
		private static readonly object CacheLock = new Object();
		private static readonly Dictionary<string, IMetric> Cache = new Dictionary<string, IMetric>();

		private static IMetricFactory Factory = new Internal.DefaultMetricFactory();

		public static void AssignFactory(IMetricFactory factory)
		{
			Factory = factory;
		}

		public static IMetric[] GetAll()
		{
			lock (CacheLock)
				return Cache.Values.ToArray();
		}

		private static IMetric New(string name, string instance, Func<IMetric> create)
		{
			IMetric retval;
			var key = name + "." + instance;

			if (!Cache.TryGetValue(key, out retval))
				lock (CacheLock)
					if (!Cache.TryGetValue(key, out retval))
						Cache[key] = retval = create();

			return retval;
		}

		public static ICounter Counter(string name, string instance)
		{
			return (ICounter)New(name, instance, () => Factory.Counter(name, instance));
		}

		public static IMeter Meter(string name, string instance, Interval interval)
		{
			return (IMeter)New(name, instance, () => Factory.Meter(name, instance, interval));
		}

		public static IGauge Gauge(string name, string instance)
		{
			return (IGauge)New(name, instance, () => Factory.Gauge(name, instance));
		}
	}

	public abstract class MetricsVisitor
	{
		public virtual void Visit(IEnumerable<IMetric> metrics)
		{
			foreach (var m in metrics)
				Visit(m);
		}

		protected virtual void Visit(IMetric metric)
		{
			var im = metric as IMeter;
			if (im != null) Visit(im);
			else
			{
				var ic = metric as ICounter;
				if (ic != null) Visit(ic);
				else
				{
					var g = metric as IGauge;
					if (g != null) Visit(g);
				}
			}
		}

		protected abstract void Visit(IMeter meter);
		protected abstract void Visit(ICounter counter);
		protected abstract void Visit(IGauge gauge);
	}

	class StringBuilderVisitor : MetricsVisitor
	{
		private StringBuilder sb;

		public StringBuilderVisitor(StringBuilder sb)
		{
			this.sb = sb;
		}

		public override void Visit(IEnumerable<IMetric> metrics)
		{
			var grouped = metrics.OrderBy(m => m.Instance).ToLookup(m => m.Name);

			foreach (var g in grouped)
			{
				sb.AppendLine(g.Key);
				sb.AppendLine("========================================");

				base.Visit(g);

				sb.AppendLine();
			}
		}

		protected override void Visit(IMetric metric)
		{
			sb.AppendFormat("{0,20}: ", metric.Instance);

			base.Visit(metric);

			sb.AppendLine("      ");
		}

		protected override void Visit(IMeter meter)
		{
			sb.AppendFormat("{0:0.##}/{1}", meter.Rate, meter.Interval.ToString().ToLower());
		}

		protected override void Visit(ICounter counter)
		{
			sb.Append(counter.Count);
		}

		protected override void Visit(IGauge gauge)
		{
			sb.AppendFormat("{0} ({1}/{2})", gauge.Value, gauge.Min, gauge.Max);
		}
	}

	public class ConsoleReporter
	{
		private StringBuilder sb;
		private StringBuilderVisitor visitor;

		public ConsoleReporter()
		{
			this.sb = new StringBuilder();
			this.visitor = new StringBuilderVisitor(sb);
		}

		public void Report()
		{
			sb.Length = 0;
			visitor.Visit(Metrics.GetAll());
			Console.CursorLeft = 0;
			Console.CursorTop = 0;
			Console.Write(sb.ToString());
		}

		public void StartPeriodicReporting(int time, Interval interval, CancellationToken cancellationToken)
		{
			var delay = (int)IntervalConverter.Convert(time, interval, Interval.Milliseconds);

			Task.Factory.StartNew(async () =>
			{
				try
				{
					while (!cancellationToken.IsCancellationRequested)
					{
						Report();

						await Task.Delay(delay, cancellationToken);
					}
				}
				catch { }
			}, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
		}
	}

	public class CsvReporter
	{
		private List<Tuple<DateTime, Tuple<string, string, double>[]>> data;
		private V visitor;

		class V : MetricsVisitor
		{

			private List<Tuple<string, string, double>> q = new List<Tuple<string, string, double>>();

			protected override void Visit(IMeter meter)
			{
				q.Add(Tuple.Create(meter.Name, meter.Instance, meter.Rate));
			}

			protected override void Visit(ICounter counter)
			{
				q.Add(Tuple.Create(counter.Name, counter.Instance, (double)counter.Count));
			}

			protected override void Visit(IGauge gauge)
			{
				q.Add(Tuple.Create(gauge.Name, gauge.Instance, (double)gauge.Value));
			}

			public Tuple<string, string, double>[] GetCurrent()
			{
				var retval = q.ToArray();
				q.Clear();

				return retval;
			}
		}


		public CsvReporter()
		{
			visitor = new V();
			data = new List<Tuple<DateTime, Tuple<string, string, double>[]>>();
		}

		public void Report()
		{
			var grouped = data
							.SelectMany(t => t.Item2, (a, b) => Tuple.Create(a.Item1, b.Item1, b.Item2, b.Item3))
							.ToLookup(t => t.Item2);

			foreach (var g in grouped)
			{
				Console.WriteLine(g.Key);
				File.WriteAllLines(g.Key.Replace("/", "_"), g.Select(t => String.Format("{0:HH:mm:ss.fffff},{1},{2:0.00}", t.Item1, t.Item3, t.Item4)));
			}
		}

		public void StartCollecting(int time, Interval interval, CancellationToken cancellationToken)
		{
			var delay = (int)IntervalConverter.Convert(time, interval, Interval.Milliseconds);

			Task.Factory.StartNew(async () =>
			{
				try
				{
					while (!cancellationToken.IsCancellationRequested)
					{
						visitor.Visit(Metrics.GetAll());
						data.Add(Tuple.Create(DateTime.Now, visitor.GetCurrent()));
						Console.WriteLine("saved");

						await Task.Delay(delay, cancellationToken);
					}
				}
				catch { }
			}, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
		}
	}

	public interface IMetricFactory
	{
		ICounter Counter(string name, string instance);
		IMeter Meter(string name, string instance, Interval interval);
		IGauge Gauge(string name, string instance);
	}

	public interface IMetric
	{
		string Name { get; }
		string Instance { get; }
	}

	public interface IGauge : IMetric
	{
		void Set(long value);

		long Value { get; }
		long Min { get; }
		long Max { get; }
	}

	public interface ICounter : IMetric
	{
		void Reset();
		void Increment();
		void Decrement();
		void IncrementBy(int value);
		void DecrementBy(int value);
		long Count { get; }
	}

	public interface IMeter : ICounter
	{
		double Rate { get; }
		Interval Interval { get; }
	}

	static class IntervalConverter
	{
		static readonly long[][] ConversionMatrix = CreateMatrix();

		static long[][] CreateMatrix()
		{
			var multipliers = new long[] { 1000L, 1000L, 1000L, 60L, 60L, 24L };
			var length = multipliers.Length + 1;
			var retval = new long[length][];

			for (var i = 0; i < length; i++)
			{
				retval[i] = new long[i];

				var tmp = 1L;
				for (var j = i - 1; j >= 0; j--)
				{
					tmp *= multipliers[j];
					retval[i][j] = tmp;
				}
			}

			return retval;
		}

		public static long Convert(long value, Interval from, Interval to)
		{
			return from == to
					? value
					: from > to
						? (value * ConversionMatrix[(int)from][(int)to])
						: (value / ConversionMatrix[(int)to][(int)from]);
		}
	}

	public enum Interval
	{
		Nanoseconds,
		Microseconds,
		Milliseconds,
		Seconds,
		Minutes,
		Hours,
		Days
	}

	namespace Internal
	{
		class DefaultMetricFactory : IMetricFactory
		{
			public ICounter Counter(string name, string instance)
			{
				return new SharedCounter(name, instance);
			}

			public IMeter Meter(string name, string instance, Interval interval)
			{
				return new DefaultMeter(name, instance, interval);
			}

			public IGauge Gauge(string name, string instance)
			{
				return new DefaultGauge(name, instance);
			}
		}

		abstract class DefaultMetric : IMetric
		{
			protected DefaultMetric(string name, string instance)
			{
				Name = name;
				Instance = instance;
			}

			public string Name { get; private set; }
			public string Instance { get; private set; }
		}

		class DefaultGauge : DefaultMetric, IGauge
		{
			public DefaultGauge(string name, string instance) : base(name, instance) { }

			public void Set(long value)
			{
				Value = value;
				if (Min > value) Min = value;
				if (Max < value) Max = value;
			}

			public long Value { get; private set; }
			public long Min { get; private set; }
			public long Max { get; private set; }
		}

		class DefaultCounter : DefaultMetric, ICounter
		{
			private long count;

			public DefaultCounter(string name, string instance) : base(name, instance) { }

			public virtual void Reset()
			{
				Interlocked.Exchange(ref count, 0);
			}

			public void Increment()
			{
				Interlocked.Increment(ref count);
			}

			public void Decrement()
			{
				Interlocked.Decrement(ref count);
			}

			public void IncrementBy(int value)
			{
				Interlocked.Add(ref count, value);
			}

			public void DecrementBy(int value)
			{
				Interlocked.Add(ref count, -value);
			}

			public long Count
			{
				get { return count; }
			}
		}

		class SharedCounter : DefaultMetric, ICounter
		{
			private long global;
			private Entry[] slices;

			private struct Entry
			{
				public int ThreadId;
				public long Value;
			}

			public SharedCounter(string name, string instance)
				: base(name, instance)
			{
				Initialize();
			}

			private void Initialize()
			{
				slices = new Entry[Environment.ProcessorCount + 16];
				global = 0;
				Thread.MemoryBarrier();
			}

			public virtual void Reset()
			{
				Initialize();
			}

			public void Increment()
			{
				ChangeBy(1);
			}

			public void Decrement()
			{
				ChangeBy(-1);
			}

			public void IncrementBy(int value)
			{
				ChangeBy(value);
			}

			public void DecrementBy(int value)
			{
				ChangeBy(-value);
			}

			private void ChangeBy(int by)
			{
				var threadId = Thread.CurrentThread.ManagedThreadId;

				for (var i = 0; i < slices.Length; i++)
				{
					var id = slices[i].ThreadId;

					if (id == threadId)
					{
						slices[i].Value = slices[i].Value + by;
						return;
					}
					else if (id == 0)
					{
						slices[i] = new Entry { ThreadId = threadId, Value = by };
						return;
					}
				}

				Interlocked.Add(ref global, by);
			}

			public long Count
			{
				get
				{
					long retval = global;
					for (var i = 0; i < slices.Length; i++)
					{
						if (slices[i].ThreadId > 0)
							retval += slices[i].Value;
					}

					return retval;
				}
			}
		}

		class DefaultMeter : SharedCounter, IMeter
		{
			private static readonly long NanoTick = 1000 * 1000 * 1000 / Stopwatch.Frequency;
			private readonly Stopwatch stopwatch;
			private readonly Interval interval;

			public DefaultMeter(string name, string instance, Interval interval)
				: base(name, instance)
			{
				this.interval = interval;
				this.stopwatch = Stopwatch.StartNew();
			}

			public override void Reset()
			{
				base.Reset();
				stopwatch.Restart();
			}

			public Interval Interval { get { return interval; } }

			public double Rate
			{
				get
				{
					var by = IntervalConverter.Convert(stopwatch.ElapsedTicks * NanoTick, Interval.Nanoseconds, interval);
					var retval = by == 0 ? 0 : Count / by;
					Reset();

					return retval;
				}
			}

			//public double Rate
			//{
			//	get
			//	{
			//		var by = IntervalConverter.Convert(stopwatch.ElapsedTicks * NanoTick, Interval.Nanoseconds, interval);

			//		return by == 0 ? 0 : Count / by;
			//	}
			//}
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
