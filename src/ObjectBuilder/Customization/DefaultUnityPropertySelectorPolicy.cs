// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ObjectBuilder2;
using Unity;
using Unity.Attributes;
using Unity.Builder.Selection;
using Unity.Policy;

namespace Microsoft.Practices.Unity.ObjectBuilder
{
    /// <summary>
    /// An implementation of <see cref="IPropertySelectorPolicy"/> that is aware of
    /// the build keys used by the unity container.
    /// </summary>
    public class DefaultUnityPropertySelectorPolicy : PropertySelectorBase<DependencyResolutionAttribute>
    {
        /// <summary>
        /// Create a <see cref="IDependencyResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected override IDependencyResolverPolicy CreateResolver(PropertyInfo property)
        {
            Microsoft.Practices.Unity.Utility.Guard.ArgumentNotNull(property, "property");

            var attributes =
                property.GetCustomAttributes(typeof(DependencyResolutionAttribute), false)
                .OfType<DependencyResolutionAttribute>()
                .ToList();

            // We must have one of these, otherwise this method would never have been called.
            Debug.Assert(attributes.Count == 1);

            return attributes[0].CreateResolver(property.PropertyType);
        }
    }
}
