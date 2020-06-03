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
        /// <param name="name">The property name.</param>
        /// <param name="value">InjectionParameterValue to use for the property.</param>
        public PropertyOverride(string name, object value)
            : base(name ?? throw new ArgumentNullException(nameof(name)), value)
        {
        }

        #endregion


        #region IEquatable

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            switch (other)
            {
                case PropertyInfo info:
                    return Equals(info);

                case PropertyOverride property:
                    return (null == Target || property.Target == Target) &&
                           (null == Name   || property.Name == Name);
                default:
                    return base.Equals(other);
            }
        }

        public bool Equals(PropertyInfo other)
        {
            return null != other && 
                  (null == Target || other.DeclaringType == Target) &&
                  (null == Name   || other.Name == Name);
        }

        #endregion
    }
}
