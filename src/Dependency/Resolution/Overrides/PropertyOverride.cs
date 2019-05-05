using System;
using System.Reflection;

namespace Unity.Resolution
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride,
                                    IEquatable<PropertyInfo>,
                                    IResolve
    {
        #region Fields

        protected readonly object Value;

        #endregion

        
        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="PropertyOverride"/>.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        /// <param name="propertyValue">InjectionParameterValue to use for the property.</param>
        public PropertyOverride(string propertyName, object propertyValue)
            : base(propertyName)
        {
            Value = propertyValue;
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
                           (null == Type   || property.Type == Type) &&
                           (null == Name   || property.Name == Name);
                default:
                    return base.Equals(other);
            }
        }

        public bool Equals(PropertyInfo other)
        {
            return null != other && 
                  (null == Target || other.DeclaringType == Target) &&
                  (null == Type   || other.PropertyType == Type) &&
                  (null == Name   || other.Name == Name);
        }

        #endregion


        #region IResolverPolicy

        public object Resolve<TContext>(ref TContext context)
            where TContext : IResolveContext
        {
            if (Value is IResolve policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory<Type> factory)
            {
                var resolveDelegate = factory.GetResolver<TContext>(Type);
                return resolveDelegate(ref context);
            }

            return Value;
        }

        #endregion
    }
}
