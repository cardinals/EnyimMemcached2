using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Funq
{
	using X = System.Linq.Expressions.Expression;

	public static class FunqExtensions
	{
		private static readonly MethodInfo ContainerResolve = typeof(Container).GetMethod("Resolve", Type.EmptyTypes);

		public static IRegistration<TService> Register<TService>(this Container container)
			where TService : class
		{
			return Register<TService>(container, typeof(TService));
		}

		public static IRegistration<TService> Register<TService, TImplementation>(this Container container)
			where TImplementation : TService
		{
			return Register<TService>(container, typeof(TImplementation));
		}

		public static IRegistration<TService> Register<TService>(this Container container, Type implType)
		{
			if (!implType.IsClass) throw new ArgumentException(implType + " must be a class");
			if (!typeof(TService).IsAssignableFrom(implType)) throw new ArgumentException(typeof(TService) + " is incompatible with " + implType);

			var ctor = implType
							.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
							.OrderByDescending(c => c.GetParameters().Length)
							.First();

			var containerParam = X.Parameter(typeof(Container), "container");
			var func = X.Lambda<Func<Container, TService>>
						(
							X.New
							(
								ctor,
								ResolveCtorArgs(ctor, containerParam).ToArray()
							),
							containerParam
						);

			return container.Register<TService>(func.Compile());
		}

		private static IEnumerable<X> ResolveCtorArgs(ConstructorInfo ctor, X instance)
		{
			foreach (var p in ctor.GetParameters())
			{
				var serviceType = p.ParameterType;
				var resolve = ResolveLazy(serviceType) ?? ResolveNormal(serviceType);

				yield return X.Call(instance, resolve);
			}
		}

		private static MethodInfo GetGenericMethod(Type type, string name, int argCount)
		{
			var retval = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
								.Where(m => m.Name == name
										&& m.IsGenericMethodDefinition
										&& m.GetGenericArguments().Length == argCount)
								.First();

			return retval;
		}

		private static MethodInfo ResolveLazy(Type serviceType)
		{
			if (!serviceType.IsGenericType) return null;

			var typeDef = serviceType.GetGenericTypeDefinition();
			if (typeDef.Assembly != typeof(Func<>).Assembly) return null;

			var genArgs = serviceType.GetGenericArguments();
			if (typeDef.FullName != "System.Func`" + genArgs.Length) return null;

			var template = GetGenericMethod(typeof(Container), "LazyResolve", genArgs.Length);
			var retval = template.MakeGenericMethod(new[] { genArgs.Last() }.Concat(genArgs.Take(genArgs.Length - 1)).ToArray());

			return retval;
		}

		private static MethodInfo ResolveNormal(Type serviceType)
		{
			return ContainerResolve.MakeGenericMethod(serviceType);
		}
	}
}