// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Injection;

namespace Unity.Utility
{
    /// <summary>
    /// Provides extension methods to the <see cref="Type"/> class due to the introduction 
    /// of <see cref="TypeInfo"/> class in the .NET for Windows Store apps.
    /// </summary>
    public static class TypeReflectionExtensions
    {
        /// <summary>
        /// Returns the non-static declared methods of a type or its base types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="MethodInfo"/> objects.</returns>
        public static IEnumerable<MethodInfo> GetMethodsHierarchical(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic);
            }

            return type.GetTypeInfo().DeclaredMethods.Where(m => !m.IsStatic)
                    .Concat(GetMethodsHierarchical(type.GetTypeInfo().BaseType));
        }

        /// <summary>
        /// Returns the declared properties of a type or its base types.
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>An enumerable of the <see cref="PropertyInfo"/> objects.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesHierarchical(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            if (type == typeof(object))
            {
                return type.GetTypeInfo().DeclaredProperties;
            }

            return type.GetTypeInfo().DeclaredProperties
                                      .Concat(GetPropertiesHierarchical(type.GetTypeInfo().BaseType));
        }

        /// <summary>
        /// Determines if the types in a parameter set ordinally matches the set of supplied types.
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="closedConstructorParameterTypes"></param>
        /// <returns></returns>
        public static bool ParametersMatch(this ParameterInfo[] parameters, Type[] closedConstructorParameterTypes)
        {
            if ((parameters ?? throw new ArgumentNullException(nameof(parameters))).Length != 
                (closedConstructorParameterTypes ?? throw new ArgumentNullException(nameof(closedConstructorParameterTypes))).Length)
            {
                return false;
            }

            return !parameters.Where((t, i) => !t.ParameterType.Equals(closedConstructorParameterTypes[i])).Any();
        }



        // TODO: Requires optimization
        /// <summary>
        /// If this type is an open generic, use the
        /// given <paramref name="genericArguments"/> array to
        /// determine what the required closed type is and return that.
        /// </summary>
        /// <remarks>If the parameter is not an open type, just
        /// return this parameter's type.</remarks>
        /// <param name="typeToReflect"></param>
        /// <param name="genericArguments">Type arguments to substitute in for
        /// the open type parameters.</param>
        /// <returns>Corresponding closed type of this parameter.</returns>
        public static Type GetClosedParameterType(this Type typeToReflect, Type[] genericArguments)
        {
            // Prior version of the framework returned both generic type arguments and parameters
            // through one mechanism, now they are broken out.  May want to consider different reflection
            // helpers instead of this case statement.

            var info = typeToReflect.GetTypeInfo();
            if (info.IsGenericType && info.ContainsGenericParameters)
            {
                Type[] typeArgs = info.IsGenericTypeDefinition ? info.GenericTypeParameters : info.GenericTypeArguments;
                for (int i = 0; i < typeArgs.Length; ++i)
                {
                    typeArgs[i] = (genericArguments ?? throw new ArgumentNullException(nameof(genericArguments)))[typeArgs[i].GenericParameterPosition];
                }
                return typeToReflect.GetGenericTypeDefinition().MakeGenericType(typeArgs);
            }

            if (info.IsGenericParameter)
            {
                return genericArguments[typeToReflect.GenericParameterPosition];
            }

            if (typeToReflect.IsArray && typeToReflect.GetElementType().GetTypeInfo().IsGenericParameter)
            {
                int rank;
                if ((rank = typeToReflect.GetArrayRank()) == 1)
                {
                    // work around to the fact that Type.MakeArrayType() != Type.MakeArrayType(1)
                    return genericArguments[typeToReflect.GetElementType().GenericParameterPosition]
                        .MakeArrayType();
                }

                return genericArguments[typeToReflect.GetElementType().GenericParameterPosition]
                    .MakeArrayType(rank);
            }

            return typeToReflect;
        }


        /// <summary>
        /// Given a generic argument name, return the corresponding type for this
        /// closed type. For example, if the current type is SomeType&lt;User&gt;, and the
        /// corresponding definition was SomeType&lt;TSomething&gt;, calling this method
        /// and passing "TSomething" will return typeof(User).
        /// </summary>
        /// <param name="type"></param>
        /// <param name="parameterName">Name of the generic parameter.</param>
        /// <returns>Type of the corresponding generic parameter, or null if there
        /// is no matching name.</returns>
        public static Type GetNamedGenericParameter(this Type type, string parameterName)
        {
            TypeInfo openType = type.GetGenericTypeDefinition().GetTypeInfo();
            Type result = null;
            int index = -1;

            foreach (var genericArgumentType in openType.GenericTypeParameters)
            {
                if (genericArgumentType.GetTypeInfo().Name == parameterName)
                {
                    index = genericArgumentType.GenericParameterPosition;
                    break;
                }
            }
            if (index != -1)
            {
                result = type.GenericTypeArguments[index];
            }
            return result;
        }


        /// <summary>
        /// Given our set of generic type arguments, 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="genericTypeArguments">The generic type arguments.</param>
        /// <returns>An array with closed parameter types. </returns>
        public static Type[] GetClosedParameterTypes(this MethodBase method, Type[] genericTypeArguments)
        {
            return method.GetParameters()
                         .Select(pi => pi.ParameterType.GetClosedParameterType(genericTypeArguments))
                         .ToArray();
        }


        /// <summary>
        /// Tests to see if the given set of types matches the ones
        /// we're looking for.
        /// </summary>
        /// <param name="parametersToMatch"></param>
        /// <param name="candidate">parameter list to look for.</param>
        /// <returns>true if they match, false if they don't.</returns>
        public static bool Matches(this IEnumerable<InjectionParameterValue> parametersToMatch, IEnumerable<Type> candidate)
        {
            var toMatch = parametersToMatch.ToArray();
            var candidateTypes = candidate.ToArray();

            if (toMatch.Length != candidateTypes.Length) return false;

            return !toMatch.Where((t, i) => !t.MatchesType(candidateTypes[i])).Any();
        }

    }
}
