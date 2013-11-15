using System;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching.Memcached.Configuration
{
	internal static class ProviderElementExtensions
	{
		public static IRegistration<TContract> RegisterInto<TContract>(this ProviderElement<TContract> element, Container target)
			where TContract : class
		{
			if (element == null) return null;

			var type = element.Type;
			if (type == null) return null;

			var reg = target.AutoWireAs<TContract>(type);

			if (typeof(ISupportInitialize).IsAssignableFrom(type))
				reg.InitializedBy((c, instance) => ((ISupportInitialize)instance).Initialize(element.Parameters));

			return reg;
		}
	}
}
