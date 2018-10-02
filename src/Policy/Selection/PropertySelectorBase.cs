using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Build;
using Unity.Builder.Selection;
using Unity.Policy;
using Unity.Utility;

namespace Unity.ObjectBuilder.BuildPlan.Selection
{
    /// <summary>
    /// Base class that provides an implementation of <see cref="IPropertySelectorPolicy"/>
    /// which lets you override how the parameter resolvers are created.
    /// </summary>
    public abstract class PropertySelectorBase<TResolutionAttribute> : IPropertySelectorPolicy
        where TResolutionAttribute : Attribute
    {
        /// <summary>
        /// Returns sequence of properties on the given type that
        /// should be set as part of building that object.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <returns>Sequence of <see cref="PropertyInfo"/> objects
        /// that contain the properties to set.</returns>
        public virtual IEnumerable<object> SelectProperties<TContext>(ref TContext context)
            where TContext : IBuildContext
        {
            Type t = context.Type;
            var list = new List<SelectedProperty>();
            foreach (PropertyInfo prop in t.GetPropertiesHierarchical().Where(p => p.CanWrite))
            {
                var propertyMethod = prop.GetSetMethod(true) ?? prop.GetGetMethod(true);
                if (propertyMethod.IsStatic)
                {
                    // Skip static properties. In the previous implementation the reflection query took care of this.
                    continue;
                }

                // Ignore indexers and return properties marked with the attribute
                if (prop.GetIndexParameters().Length == 0 &&
                   prop.IsDefined(typeof(TResolutionAttribute), false))
                {
                    list.Add(CreateSelectedProperty(prop));
                }
            }

            return list;
        }

        private SelectedProperty CreateSelectedProperty(PropertyInfo property)
        {
            IResolverPolicy resolver = CreateResolver(property);
            return new SelectedProperty(property, resolver);
        }

        /// <summary>
        /// Create a <see cref="IResolverPolicy"/> for the given
        /// property.
        /// </summary>
        /// <param name="property">Property to create resolver for.</param>
        /// <returns>The resolver object.</returns>
        protected abstract IResolverPolicy CreateResolver(PropertyInfo property);
    }
}
