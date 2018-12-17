using System;
using System.Reflection;

namespace Unity.Builder
{
    /// <summary>
    /// Objects of this type are returned from
    /// <see cref="IPropertySelectorPolicy.SelectProperties"/>.
    /// This class combines the <see cref="PropertyInfo"/> about
    /// the property with the string key used to look up the resolver
    /// for this property's value.
    /// </summary>
    public class SelectedProperty : IEquatable<PropertyInfo>
    {
        /// <summary>
        /// Create an instance of <see cref="SelectedProperty"/>
        /// with the given <see cref="PropertyInfo"/> and key.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="resolver"></param>
        public SelectedProperty(PropertyInfo property, object resolver)
        {
            Property = property;
            Resolver = resolver;
        }

        /// <summary>
        /// PropertyInfo for this property.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// IResolverPolicy for this property
        /// </summary>
        public object Resolver { get; }

        public bool Equals(PropertyInfo other)
        {
            return Property?.Equals(other) ?? false;
        }


        #region Overrides

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case PropertyInfo info:
                    return Equals(info);

                case IEquatable<PropertyInfo> equatable:
                    return equatable.Equals(Property);

                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return Property?.GetHashCode() ?? 0;
        }

        #endregion
    }
}
