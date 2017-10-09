// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Builder;
using Unity.Policy;

namespace Unity.ResolverPolicy
{
    /// <summary>
    /// An implementation of <see cref="IDependencyResolverPolicy"/> that resolves to
    /// to an array populated with the values that result from resolving other instances
    /// of <see cref="IDependencyResolverPolicy"/>.
    /// </summary>
    public class ResolvedArrayWithElementsResolverPolicy : IDependencyResolverPolicy
    {
        private delegate object Resolver(IBuilderContext context, IDependencyResolverPolicy[] elementPolicies);
        private readonly Resolver _resolver;
        private readonly IDependencyResolverPolicy[] _elementPolicies;

        /// <summary>
        /// Create an instance of <see cref="ResolvedArrayWithElementsResolverPolicy"/>
        /// with the given type and a collection of <see cref="IDependencyResolverPolicy"/>
        /// instances to use when populating the result.
        /// </summary>
        /// <param name="elementType">The type.</param>
        /// <param name="elementPolicies">The resolver policies to use when populating an array.</param>
        public ResolvedArrayWithElementsResolverPolicy(Type elementType, params IDependencyResolverPolicy[] elementPolicies)
        {

            var resolverMethodInfo
                = typeof(ResolvedArrayWithElementsResolverPolicy)
                    .GetTypeInfo().GetDeclaredMethod("DoResolve")
                        .MakeGenericMethod(elementType ?? throw new ArgumentNullException(nameof(elementType)));

            _resolver = (Resolver)resolverMethodInfo.CreateDelegate(typeof(Resolver));

            _elementPolicies = elementPolicies;
        }

        /// <summary>
        /// Resolve the value for a dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>An array populated with the results of resolving the resolver policies.</returns>
        public object Resolve(IBuilderContext context)
        {
            return _resolver(context ?? throw new ArgumentNullException(nameof(context)), _elementPolicies);
        }

        // TODO: Remove unused
        private static object DoResolve<T>(IBuilderContext context, IDependencyResolverPolicy[] elementPolicies)
        {
            T[] result = new T[elementPolicies.Length];

            for (int i = 0; i < elementPolicies.Length; i++)
            {
                result[i] = (T)elementPolicies[i].Resolve(context);
            }

            return result;
        }
    }
}
