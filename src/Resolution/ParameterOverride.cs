// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Builder.Operation;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that lets you
    /// override a named parameter passed to a constructor.
    /// </summary>
    public class ParameterOverride : ResolverOverride
    {
        private readonly string _parameterName;
        private readonly InjectionParameterValue _parameterValue;

        /// <summary>
        /// Construct a new <see cref="ParameterOverride"/> object that will
        /// override the given named constructor parameter, and pass the given
        /// value.
        /// </summary>
        /// <param name="parameterName">Name of the constructor parameter.</param>
        /// <param name="parameterValue">Value to pass for the constructor.</param>
        public ParameterOverride(string parameterName, object parameterValue)
        {
            _parameterName = parameterName;
            _parameterValue = InjectionParameterValue.ToParameter(parameterValue);
        }

        /// <summary>
        /// Return a <see cref="IDependencyResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IDependencyResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            if ((context ?? throw new ArgumentNullException(nameof(context))).CurrentOperation is ParameterResolveOperation currentOperation &&
                currentOperation.ParameterName == _parameterName)
            {
                return _parameterValue.GetResolverPolicy(dependencyType);
            }

            return null;
        }
    }
}
