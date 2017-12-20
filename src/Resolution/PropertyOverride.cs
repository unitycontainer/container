// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using Unity.Builder;
using Unity.Builder.Operation;
using Unity.Injection;
using Unity.Policy;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride
    {
        /// <summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">Value to use for the property.</param>
        public PropertyOverride(string propertyName, object propertyValue)
            : base(propertyName, propertyValue ?? throw new ArgumentNullException(nameof(propertyValue)))
        {
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            if ((context ?? throw new ArgumentNullException(nameof(context))).CurrentOperation is ResolvingPropertyValueOperation currentOperation
                && currentOperation.PropertyName == Name)
            {
                return Value.GetResolverPolicy(dependencyType);
            }
            return null;
        }
    }
}
