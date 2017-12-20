// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reflection;
using Unity.Builder.Selection;
using Unity.Policy;
using Unity.ResolverPolicy;

namespace Unity.ObjectBuilder.BuildPlan.Selection
{
    /// <summary>
    /// An implementation of <see cref="IPropertySelectorPolicy"/> that looks
    /// for properties marked with the <typeparamref name="TResolutionAttribute"/>
    /// attribute that are also settable and not indexers.
    /// </summary>
    /// <typeparam name="TResolutionAttribute"></typeparam>
    public class PropertySelectorPolicy<TResolutionAttribute> : PropertySelectorBase<TResolutionAttribute>
        where TResolutionAttribute : Attribute
    {
        /// <summary>
        /// Create a <see cref="IResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IResolverPolicy CreateResolver(PropertyInfo property)
        {
            return new FixedTypeResolverPolicy((property ?? throw new ArgumentNullException(nameof(property))).PropertyType);
        }
    }
}
