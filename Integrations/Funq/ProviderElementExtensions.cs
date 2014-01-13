using System;
using Enyim.Caching.Configuration;
using Funq;

namespace Enyim.Caching
{
	static partial class ProviderElementExtensions
	{
		public static IRegistration<TContract> TryRegisterInto<TContract>(this ProviderElement<TContract> element, Container target)
			where TContract : class
		{
			return element.RegisterInto(target, null);
		}

		public static IRegistration<TContract> RegisterInto<TContract>(this ProviderElement<TContract> element, Container target, Type defaultImplementation)
			where TContract : class
		{
			var type = element == null || element.Type == null
						? defaultImplementation
						: element.Type;

			if (type == null) return null;

			var reg = target.AutoWireAs<TContract>(type);
			if (typeof(ISupportInitialize).IsAssignableFrom(type))
				reg.InitializedBy((c, instance) => ((ISupportInitialize)instance).Initialize(element.Parameters));

			return reg;
		}
	}
}
