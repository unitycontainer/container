// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using Unity.Attributes;
using Unity.ObjectBuilder.BuildPlan.Selection;
using Unity.Policy;
using Unity.ResolverPolicy;

namespace Unity.ObjectBuilder.Policies
{
    /// <summary>
    /// An implementation of <see cref="IMethodSelectorPolicy"/> that is aware
    /// of the build keys used by the Unity container.
    /// </summary>
    public class DefaultUnityMethodSelectorPolicy : MethodSelectorPolicyBase<InjectionMethodAttribute>
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> instance for the given
        /// <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">Parameter to create the resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IDependencyResolverPolicy CreateResolver(ParameterInfo parameter)
        {
            var attributes = (parameter ?? throw new ArgumentNullException(nameof(parameter))).GetCustomAttributes(false)
                .OfType<DependencyResolutionAttribute>()
                .ToList();

            if (attributes.Count > 0)
            {
                return attributes[0].CreateResolver(parameter.ParameterType);
            }

            return new NamedTypeDependencyResolverPolicy(parameter.ParameterType, null);
        }
    }
}
