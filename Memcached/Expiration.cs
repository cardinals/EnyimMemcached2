using System;

namespace Enyim.Caching.Memcached
{
	public struct Expiration : IEquatable<Expiration>
	{
		private const int MaxSeconds = 60 * 60 * 24 * 30;
		private static readonly DateTime UnixEpochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		public static readonly Expiration Never = new Expiration(0);

		public Expiration(uint value)
		{
			Value = value;
			IsForever = value == 0 || value == UInt32.MaxValue;
			IsAbsolute = value >= MaxSeconds || IsForever;
		}

		public bool IsAbsolute { get; private set; }
		public bool IsForever { get; private set; }
		public bool IsNever { get { return Value == 0; } }
		public uint Value { get; private set; }

		public override int GetHashCode()
		{
			return HashCodeCombiner.Combine((int)Value, IsAbsolute ? 0 : 1, IsForever ? 0 : 1);
		}

		public override bool Equals(object obj)
		{
			return Equals((Expiration)obj);
		}

		public bool Equals(Expiration other)
		{
			return Value == other.Value
					&& IsAbsolute == other.IsAbsolute
					&& IsForever == other.IsForever;
		}

		public static Expiration From(TimeSpan validFor)
		{
			// infinity
			if (validFor == TimeSpan.Zero || validFor == TimeSpan.MaxValue)
				return Never;

			var seconds = (uint)validFor.TotalSeconds;
			if (seconds < MaxSeconds) return new Expiration { Value = seconds };

			return From(SystemTime.Now() + validFor);
		}

		public static Expiration From(DateTime expiresAt)
		{
			if (expiresAt == DateTime.MaxValue || expiresAt == DateTime.MinValue)
				return Never;

			if (expiresAt <= UnixEpochUtc)
				throw new ArgumentOutOfRangeException("expiresAt must be > " + UnixEpochUtc);

			return new Expiration
			{
				IsAbsolute = true,
				Value = (uint)(expiresAt.ToUniversalTime() - UnixEpochUtc).TotalSeconds
			};
		}

		public static implicit operator Expiration(TimeSpan validFor) => From(validFor);
		public static implicit operator Expiration(DateTime expiresAt) => From(expiresAt);
		public static implicit operator Expiration(uint value) => new Expiration(value);

		public static bool operator ==(Expiration a, Expiration b) => a.Equals(b);
		public static bool operator !=(Expiration a, Expiration b) => !a.Equals(b);
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
