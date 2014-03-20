using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;

namespace Enyim.Caching.Memcached.Configuration
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IFluentSyntax
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		Type GetType();

		[EditorBrowsable(EditorBrowsableState.Never)]
		int GetHashCode();

		[EditorBrowsable(EditorBrowsableState.Never)]
		string ToString();

		[EditorBrowsable(EditorBrowsableState.Never)]
		bool Equals(object obj);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHaveServices<out TServices> : IFluentSyntax
	{
		TServices Add { get; }
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IHaveName
	{
		string Name { get; }
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICanAddServices<out TNext> : IFluentSyntax
	{
		TNext Service<TService>(Func<TService> factory);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClusterBuilder : IHaveName
	{
		IClusterBuilderNext Endpoints(IEnumerable<IPEndPoint> endpoints);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface ICanBeRegistered : IFluentSyntax
	{
		void Register();
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClusterBuilderNext : IHaveServices<IClusterBuilderServices>, ICanBeRegistered { }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClusterBuilderServices : ICanAddServices<IClusterBuilderServicesNext> { }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClusterBuilderServicesNext : IClusterBuilderServices, ICanBeRegistered { }


	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClientBuilderServices : ICanAddServices<IClientBuilderServicesNext> { }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClientBuilderServicesNext : IClientBuilderServices, IClientConfigurationBuilderDefaults { }

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClientConfigurationBuilderDefaults : IHaveServices<IClientBuilderServices>
	{
		IContainer Create();
		void MakeDefault(bool force = false);
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IClientConfigurationBuilder : IClientConfigurationBuilderDefaults
	{
		IClientConfigurationBuilderDefaults Cluster(string name);
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
