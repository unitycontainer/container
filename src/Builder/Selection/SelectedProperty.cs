// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Reflection;
using Unity.Policy;

namespace Unity.Builder.Selection
{
    /// <summary>
    /// Objects of this type are returned from
    /// <see cref="IPropertySelectorPolicy.SelectProperties"/>.
    /// This class combines the <see cref="PropertyInfo"/> about
    /// the property with the string key used to look up the resolver
    /// for this property's value.
    /// </summary>
    public class SelectedProperty
    {
        /// <summary>
        /// Create an instance of <see cref="SelectedProperty"/>
        /// with the given <see cref="PropertyInfo"/> and key.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="resolver"></param>
        public SelectedProperty(PropertyInfo property, IDependencyResolverPolicy resolver)
        {
            Property = property;
            Resolver = resolver;
        }

        /// <summary>
        /// PropertyInfo for this property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// IDependencyResolverPolicy for this property
        /// </summary>
        public IDependencyResolverPolicy Resolver { get; }
    }
}
