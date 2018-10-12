using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.Policy;
using Unity.Registration;
using Unity.ResolverPolicy;
using Unity.Utility;

namespace Unity.ObjectBuilder.Policies
{
    /// <summary>
    /// An implementation of <see cref="IPropertySelectorPolicy"/> that is aware of
    /// the build keys used by the unity container.
    /// </summary>
    public class DefaultUnityPropertySelectorPolicy : IPropertySelectorPolicy
    {
        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>Sequence of <see cref="PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        public virtual IEnumerable<object> SelectProperties<TBuilderContext>(ref TBuilderContext context)
            where TBuilderContext : IBuilderContext
        {
            var properties = context.Type
                                    .GetPropertiesHierarchical()
                                    .Where(p => p.CanWrite);

            var injectionMembers = 
                context.Registration is InternalRegistration registration && null != registration.InjectionMembers 
                    ? registration.InjectionMembers 
                    : null;

            return null != injectionMembers
                ? SelectInjectedProperties(properties, injectionMembers.OfType<IEquatable<PropertyInfo>>()
                                                                       .ToArray())
                : SelectAttributedProperties(properties);
        }

        private IEnumerable<object> SelectAttributedProperties(IEnumerable<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                var propertyMethod = property.GetSetMethod(true) ??
                                     property.GetGetMethod(true);

                // Skip static properties and indexers. 
                if (propertyMethod.IsStatic ||
                    property.GetIndexParameters().Length != 0) continue;

                // Return properties marked with the attribute
                if (property.IsDefined(typeof(DependencyResolutionAttribute), false))
                {
                    yield return new SelectedProperty(property, CreateResolver(property));
                }
            }
        }

        private IEnumerable<object> SelectInjectedProperties(IEnumerable<PropertyInfo> properties, 
            IEquatable<PropertyInfo>[] injectionMembers)
        {
            foreach (var property in properties)
            {
                var propertyMethod = property.GetSetMethod(true) ??
                                     property.GetGetMethod(true);

                // Skip static properties and indexers. 
                if (propertyMethod.IsStatic ||
                    property.GetIndexParameters().Length != 0) continue;

                var injector = injectionMembers.Where(member => member.Equals(property))
                                               .Select(member => new SelectedProperty(property, member))
                                               .FirstOrDefault() 
                               ?? (property.IsDefined(typeof(DependencyResolutionAttribute), false)
                                   ? new SelectedProperty(property, CreateResolver(property)) 
                                   : null);

                if (null != injector) yield return injector;
            }
        }


        /// <summary>
        /// Create a <see cref="IResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected IResolverPolicy CreateResolver(PropertyInfo property)
        {
            var attribute = property.GetCustomAttributes(typeof(DependencyResolutionAttribute), false)
                                    .OfType<DependencyResolutionAttribute>()
                                    .First();

            return attribute is OptionalDependencyAttribute dependencyAttribute
                ? (IResolverPolicy)new OptionalDependencyResolverPolicy(property.PropertyType, dependencyAttribute.Name)
                : null != attribute.Name 
                    ? new NamedTypeDependencyResolverPolicy(property.PropertyType, attribute.Name) 
                    : null;
        }
    }
}
