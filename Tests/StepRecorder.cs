using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Enyim.Caching.Tests
{
	internal class StepRecorder
	{
		private List<string> steps = new List<string>();

		public void Mark(string step)
		{
			steps.Add(step);
		}

		public void AssertSteps(params string[] expected)
		{
			AssertSteps(expected.AsEnumerable());
		}

		public void AssertSteps(IEnumerable<string> expected)
		{
			Assert.Empty(expected.Except(steps));
			Assert.Empty(steps.Except(expected));
		}
	}
}
