using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching
{
	internal static class Require
	{
		public static void Value(string name, bool valid, string message)
		{
			if (!valid)
				throw new ArgumentOutOfRangeException(name, message);
		}

		public static void That(bool condition, string message)
		{
			if (!condition)
				throw new InvalidOperationException(message);
		}

		public static void NotNull(object value, string parameter, string message = null)
		{
			if (value == null)
				throw new ArgumentNullException(parameter, message);
		}
	}
}
