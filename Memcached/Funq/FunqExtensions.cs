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

		#region [ AutoWire/AutoWireAs          ]

		public static IRegistration<TService> AutoWireAs<TService>(this Container container, Type implementationType)
			where TService : class
		{
			return Register<TService>(container, implementationType);
		}

		public static IRegistration<TService> AutoWire<TService>(this Container container)
			where TService : class
		{
			return Register<TService>(container, typeof(TService));
		}

		public static IRegistration<TService> AutoWire<TService, TArg1>(this Container container)
			where TService : class
		{
			return Register<TService>(container, typeof(TService), typeof(TArg1));
		}

		public static IRegistration<TService> AutoWire<TService, TArg1, TArg2>(this Container container)
			where TService : class
		{
			return Register<TService>(container, typeof(TService), typeof(TArg1), typeof(TArg2));
		}

		public static IRegistration<TService> AutoWire<TService, TArg1, TArg2, TArg3>(this Container container)
			where TService : class
		{
			return Register<TService>(container, typeof(TService), typeof(TArg1), typeof(TArg2), typeof(TArg3));
		}

		public static IRegistration<TService> AutoWire<TService, TArg1, TArg2, TArg3, TArg4>(this Container container)
			where TService : class
		{
			return Register<TService>(container, typeof(TService), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4));
		}

		public static IRegistration<TService> AutoWireAs<TService, TImplementation>(this Container container)
			where TImplementation : class, TService
		{
			return Register<TService>(container, typeof(TImplementation));
		}

		public static IRegistration<TService> AutoWireAs<TService, TImplementation, TArg1>(this Container container)
			where TImplementation : class, TService
		{
			return Register<TService>(container, typeof(TImplementation), typeof(TArg1));
		}

		public static IRegistration<TService> AutoWireAs<TService, TImplementation, TArg1, TArg2>(this Container container)
			where TImplementation : class, TService
		{
			return Register<TService>(container, typeof(TImplementation), typeof(TArg1), typeof(TArg2));
		}

		public static IRegistration<TService> AutoWireAs<TService, TImplementation, TArg1, TArg2, TArg3>(this Container container)
			where TImplementation : class, TService
		{
			return Register<TService>(container, typeof(TImplementation), typeof(TArg1), typeof(TArg2), typeof(TArg3));
		}

		public static IRegistration<TService> AutoWireAs<TService, TImplementation, TArg1, TArg2, TArg3, TArg4>(this Container container)
			where TImplementation : class, TService
		{
			return Register<TService>(container, typeof(TImplementation), typeof(TArg1), typeof(TArg2), typeof(TArg3), typeof(TArg4));
		}

		#endregion

		private static IRegistration<TService> Register<TService>(this Container container, Type implType, params Type[] openArgs)
		{
			if (!implType.IsClass) throw new ArgumentException(implType + " must be a class");
			if (!typeof(TService).IsAssignableFrom(implType)) throw new ArgumentException(typeof(TService) + " is incompatible with " + implType);

			// IRegistration<TService> Register<TService, TArg>(Func<Container, TArg, TService> factory)
			var funcType = CreateFactoryFunc(typeof(TService), openArgs);

			// build the parameters
			var containerParam = X.Parameter(typeof(Container), "container");
			var openParams = openArgs.Select((t, i) => X.Parameter(t, "open_" + i)).ToArray();

			// find the appropriate constructor
			int startAt;
			var ctor = MatchCtor(implType, openArgs, out startAt);

			var func = X.Lambda
						(
							funcType,
							X.New
							(
								ctor,

								// ctor args BEFORE the openArgs -> must be resolved from container
								GenerateResolveCalls(containerParam, ctor.GetParameters().Take(startAt))
				// open args -> provided by caller
									.Concat(openParams)
				// ctor args AFTER the openArgs -> must be resolved from container
									.Concat(GenerateResolveCalls(containerParam, ctor.GetParameters().Skip(startAt + openArgs.Length)))
									.ToArray()
							),
							new[] { containerParam }.Concat(openParams).ToArray()
						);

			var register = GetGenericMethod(typeof(Container), "Register", openArgs.Length + 1, 1)
								.MakeGenericMethod(new[] { typeof(TService) }.Concat(openArgs).ToArray());

			return (IRegistration<TService>)register.Invoke(container, new object[] { func.Compile() });
		}
		
		/// <summary>
		/// Finds the longest ctor of the type which has parameters either starting or ending with the specified list of types.
		/// </summary>
		private static ConstructorInfo MatchCtor(Type owner, Type[] openArgs, out int openStartsAt)
		{
			openStartsAt = 0;
			var all = owner.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
							.OrderByDescending(c => c.GetParameters().Length);

			if (openArgs.Length == 0) return all.First();

			foreach (var ctor in all)
			{
				var ctorParams = ctor.GetParameters().Select(p => p.ParameterType).ToArray();
				if (!ctorParams.Intersect(openArgs).SequenceEqual(openArgs)) continue;

				for (var i = 0; i < ctorParams.Length; i++)
				{
					if (ctorParams.Skip(i).Take(openArgs.Length).SequenceEqual(openArgs))
					{
						openStartsAt = i;

						return ctor;
					}
				}
			}

			throw new InvalidOperationException(".ctor not found with sub-list of params: " + String.Join<Type>(", ", openArgs));
		}

		/// <summary>
		/// create factory Func<Container, Arg1..ArgN, TService> 
		/// </summary>
		private static Type CreateFactoryFunc(Type serviceType, Type[] openArgs)
		{
			var funcArgs = new List<Type>();
			funcArgs.Add(typeof(Container));
			funcArgs.AddRange(openArgs);
			funcArgs.Add(serviceType);

			// IRegistration<TService> Register<TService, TArg>(Func<Container, TArg, TService> factory)
			var funcType = Type
								.GetType("System.Func`" + (openArgs.Length + 2) + ", mscorlib")
								.MakeGenericType(funcArgs.ToArray());

			return funcType;
		}

		/// <summary>
		/// Generates Resolve/LazyResolve calls based on the parameters
		/// </summary>
		private static IEnumerable<X> GenerateResolveCalls(X container, IEnumerable<ParameterInfo> parameters)
		{
			foreach (var p in parameters)
			{
				var serviceType = p.ParameterType;
				var resolve = GetResolveLazy(serviceType) ?? GetResolveNormal(serviceType);

				yield return X.Call(container, resolve);
			}
		}

		private static MethodInfo GetGenericMethod(Type type, string name, int argCount, int paramCount)
		{
			var retval = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
								.Where(m => m.Name == name
											&& m.IsGenericMethodDefinition
											&& m.GetGenericArguments().Length == argCount
											&& m.GetParameters().Length == paramCount)
								.First();

			return retval;
		}

		private static MethodInfo GetResolveLazy(Type serviceType)
		{
			if (!serviceType.IsGenericType) return null;

			var typeDef = serviceType.GetGenericTypeDefinition();
			if (typeDef.Assembly != typeof(Func<>).Assembly) return null;

			var genArgs = serviceType.GetGenericArguments();
			if (typeDef.FullName != "System.Func`" + genArgs.Length) return null;

			// Get LazyResolve<,,,>();
			var template = GetGenericMethod(typeof(Container), "LazyResolve", genArgs.Length, 0);
			var retval = template.MakeGenericMethod(new[] { genArgs.Last() }.Concat(genArgs.Take(genArgs.Length - 1)).ToArray());

			return retval;
		}

		private static MethodInfo GetResolveNormal(Type serviceType)
		{
			return ContainerResolve.MakeGenericMethod(serviceType);
		}
	}
}