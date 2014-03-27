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
