using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Funq
{
	using X = System.Linq.Expressions.Expression;

	internal static partial class FunqExtensions
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

#if AUTO_OPEN_ARGS
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
#endif
		public static IRegistration<TService> AutoWireAs<TService, TImplementation>(this Container container)
			where TImplementation : class, TService
		{
			return Register<TService>(container, typeof(TImplementation));
		}

#if AUTO_OPEN_ARGS
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
#endif
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

								/*
									ctor args BEFORE the openArgs -> must be resolved from container
									open args -> provided by caller
									ctor args AFTER the openArgs -> must be resolved from container
								*/
								GenerateResolveCalls(containerParam, ctor.GetParameters().Take(startAt))
									.Concat(openParams)
									.Concat(GenerateResolveCalls(containerParam, ctor.GetParameters().Skip(startAt + openArgs.Length)))
									.ToArray()
							),
							new[] { containerParam }.Concat(openParams).ToArray()
						);

			var register = FindRegister(openArgs.Length)
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

			// IRegistration<TService> Register<TService, TArg, TArg, ...>(Func<Container, TArg, TArg, ..., TService> factory)
			return X.GetFuncType(funcArgs.ToArray());
		}

		/// <summary>
		/// Generates Resolve/LazyResolve calls based on the parameters
		/// </summary>
		private static IEnumerable<X> GenerateResolveCalls(X container, IEnumerable<ParameterInfo> parameters)
		{
			foreach (var p in parameters)
			{
				var serviceType = p.ParameterType;
				var resolve = MakeLazyResolve(serviceType) ?? MakeResolve(serviceType);

				yield return X.Call(container, resolve);
			}
		}

		internal static MethodInfo FindRegister(int openArgLength)
		{
			var genericArgCount = openArgLength + 1;
			var func = "System.Func`" + (genericArgCount + 1);

			var query = (from m in typeof(Container).GetMethods(BindingFlags.Public | BindingFlags.Instance)
						 let p = m.GetParameters()
						 let p0 = p.Length == 1 ? p[0].ParameterType : null
						 where m.Name == "Register"
								&& m.IsGenericMethodDefinition
								&& m.GetGenericArguments().Length == genericArgCount
								&& p0 != null && p0.IsGenericType
								&& p0.GetGenericTypeDefinition().FullName == func
						 select m);

			return query.Single();
		}

		internal static MethodInfo FindLazyResolve(int genericArgCount)
		{
			var query = typeof(Container)
								.GetMethods(BindingFlags.Public | BindingFlags.Instance)
								.Where(m => m.Name == "LazyResolve"
											&& m.IsGenericMethodDefinition
											&& m.GetGenericArguments().Length == genericArgCount
											&& m.GetParameters().Length == 0);

			return query.Single();
		}

		internal static MethodInfo FindResolve(int parameterCount)
		{
			var query = (from m in typeof(Container).GetMethods(BindingFlags.Public | BindingFlags.Instance)
						 let p = m.GetParameters()
						 where m.Name == "Resolve"
								&& m.IsGenericMethodDefinition
								&& m.GetGenericArguments().Length == parameterCount + 1
								&& p.Length == parameterCount
						 select m);

			return query.Single();
		}

		private static MethodInfo MakeLazyResolve(Type serviceType)
		{
			if (!serviceType.IsGenericType) return null;

			var typeDef = serviceType.GetGenericTypeDefinition();
			if (typeDef.Assembly != typeof(Func<>).Assembly) return null;

			var genArgs = serviceType.GetGenericArguments();
			if (typeDef.FullName != "System.Func`" + genArgs.Length) return null;

			// Get LazyResolve<,,,>();
			var template = FindLazyResolve(genArgs.Length);
			var retval = template.MakeGenericMethod(new[] { genArgs.Last() }.Concat(genArgs.Take(genArgs.Length - 1)).ToArray());

			return retval;
		}

		private static MethodInfo MakeResolve(Type serviceType)
		{
			return ContainerResolve.MakeGenericMethod(serviceType);
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
