using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Policy;
using Unity.ResolverPolicy;
using Unity.Utility;

namespace Unity.Builder
{
    /// <summary>
    /// An implementation of <see cref="IPropertySelectorPolicy"/> that is aware of
    /// the build keys used by the unity container.
    /// </summary>
    public class ImportedPropertiesSelector : MemberSelectorPolicy<PropertyInfo, object>, 
                                                      IPropertySelectorPolicy
    {
        #region IPropertySelectorPolicy

        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>Sequence of <see cref="PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        public IEnumerable<object> SelectProperties<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext 
            => Select(ref context); 

        #endregion


        #region Overrides

        protected override IEnumerable<object> GetAttributedMembers(Type type)
        {
            var properties = type.GetPropertiesHierarchical()
#if NETSTANDARD1_0
                                .Where(p =>
                                {
                                    if (!p.CanWrite) return false;

                                    var propertyMethod = p.GetSetMethod(true) ??
                                                         p.GetGetMethod(true);

                                    // Skip static properties and indexers. 
                                    if (propertyMethod.IsStatic || p.GetIndexParameters().Length != 0)
                                        return false;

                                    return true;
                                })
#else
                                .Where(p => p.CanWrite && !p.SetMethod.IsStatic && p.GetIndexParameters().Length == 0)
#endif
                                .ToList();

            foreach (var property in properties)
            {
                // Return properties marked with the attribute
                if (property.IsDefined(typeof(DependencyResolutionAttribute), false))
                {
                    yield return new SelectedProperty(property, CreateResolver(property));
                }
            }
        }

        #endregion

        /// <summary>
        /// Create a <see cref="IResolve"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected IResolve CreateResolver(PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes(typeof(DependencyResolutionAttribute), false)
                                    .OfType<DependencyResolutionAttribute>()
                                    .First();

            return attribute is OptionalDependencyAttribute dependencyAttribute
                ? (IResolve)new OptionalDependencyResolvePolicy(property.PropertyType, dependencyAttribute.Name)
                : null != attribute.Name
                    ? new NamedTypeDependencyResolvePolicy(property.PropertyType, attribute.Name)
                    : null;
        }
    }
}
