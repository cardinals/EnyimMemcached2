using System;
using System.Collections.Generic;
using Enyim.Caching;
using Enyim.Caching.Memcached;
using Moq;
using Xunit;

namespace Enyim.Caching.Tests
{
	public class PeriodicReconnectPolicyTests
	{
		[Fact]
		public void Returns_Proper_Interval()
		{
			var interval = TimeSpan.FromSeconds(20);
			var policy = new PeriodicReconnectPolicy { Interval = interval };

			var node1 = new Mock<INode>().Object;
			var node2 = new Mock<INode>().Object;
			
			Assert.Equal(policy.Schedule(node1), interval);
			Assert.Equal(policy.Schedule(node2), interval);

			policy.Reset(node1);
			policy.Reset(node2);

			Assert.Equal(policy.Schedule(node1), interval);
			Assert.Equal(policy.Schedule(node2), interval);
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
