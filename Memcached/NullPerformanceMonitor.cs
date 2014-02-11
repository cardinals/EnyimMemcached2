using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enyim.Caching.Memcached
{
	public class NullPerformanceMonitor : IPerformanceMonitor
	{
		public void Get(int amount, bool success) { }
		public void Store(StoreMode mode, int amount, bool success) { }
		public void Delete(int amount, bool success) { }
		public void Mutate(MutationMode mode, int amount, bool success) { }
		public void Concatenate(ConcatenationMode mode, int amount, bool success) { }
		public void Dispose() { }
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
