using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride,
                                    IEquatable<PropertyInfo>
    {
        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">InjectionParameterValue to use for the property.</param>
        public PropertyOverride(string propertyName, object propertyValue)
            : base(propertyName, propertyValue ?? throw new ArgumentNullException(nameof(propertyValue)))
        {
        }

        #endregion


        #region IEquatable

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyInfo info)
            {
                return (null == Target || info.DeclaringType == Target) &&
                       (null == Type   || info.PropertyType == Type) &&
                       (null == Name   || info.Name == Name);
            }

            return base.Equals(obj);
        }

        public bool Equals(PropertyInfo other)
        {
            return (null == Target || other.DeclaringType == Target) &&
                   (null == Type   || other.PropertyType == Type) &&
                   (null == Name   || other.Name == Name);
        }

        #endregion
    }
}
