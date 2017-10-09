// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Builder.Selection;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Builder.Policy
{
    /// <summary>
    /// An implementation of <see cref="IConstructorSelectorPolicy"/> that selects
    /// the given constructor and creates the appropriate resolvers to call it with
    /// the specified parameters.
    /// </summary>
    public class SpecifiedConstructorSelectorPolicy : IConstructorSelectorPolicy
    {
        private readonly ConstructorInfo _ctor;
        private readonly MethodReflectionHelper _ctorReflector;
        private readonly InjectionParameterValue[] _parameterValues;

        /// <summary>
        /// Create an instance of <see cref="SpecifiedConstructorSelectorPolicy"/> that
        /// will return the given constructor, being passed the given injection values
        /// as parameters.
        /// </summary>
        /// <param name="ctor">The constructor to call.</param>
        /// <param name="parameterValues">Set of <see cref="InjectionParameterValue"/> objects
        /// that describes how to obtain the values for the constructor parameters.</param>
        public SpecifiedConstructorSelectorPolicy(ConstructorInfo ctor, InjectionParameterValue[] parameterValues)
        {
            _ctor = ctor;
            _ctorReflector = new MethodReflectionHelper(ctor);
            _parameterValues = parameterValues;
        }

        /// <summary>
        /// Choose the constructor to call for the given type.
        /// </summary>
        /// <param name="context">Current build context</param>
        /// <param name="resolverPolicyDestination">The <see cref='IPolicyList'/> to add any
        /// generated resolver objects into.</param>
        /// <returns>The chosen constructor.</returns>
        public SelectedConstructor SelectConstructor(IBuilderContext context, IPolicyList resolverPolicyDestination)
        {
            SelectedConstructor result;
            Type typeToBuild = (context ?? throw new ArgumentNullException(nameof(context))).BuildKey.Type;

            ReflectionHelper typeReflector = new ReflectionHelper(_ctor.DeclaringType);
            if (!_ctorReflector.MethodHasOpenGenericParameters && !typeReflector.IsOpenGeneric)
            {
                result = new SelectedConstructor(_ctor);
            }
            else
            {
                Type[] closedCtorParameterTypes =
                    _ctorReflector.GetClosedParameterTypes(typeToBuild.GetTypeInfo().GenericTypeArguments);

                result = new SelectedConstructor(typeToBuild.GetConstructor(closedCtorParameterTypes));
            }

            foreach (InjectionParameterValue parameterValue in _parameterValues)
            {
                var resolver = parameterValue.GetResolverPolicy(typeToBuild);
                result.AddParameterResolver(resolver);
            }

            return result;
        }
    }
}
