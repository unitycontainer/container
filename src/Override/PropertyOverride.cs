using System;
using System.Reflection;
using Unity.Build;
using Unity.Factory;
using Unity.Policy;

namespace Unity
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> that lets you override
    /// the value for a specified property.
    /// </summary>
    public class PropertyOverride : ResolverOverride,
                                    IEquatable<PropertyInfo>,
                                    IResolverPolicy
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
            Value = propertyValue ?? throw new ArgumentNullException(nameof(propertyValue));
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
                return Equals(info);

            return base.Equals(obj);
        }

        public bool Equals(PropertyInfo other)
        {
            return (null == Target || other?.DeclaringType == Target) &&
                   (null == Type   || other?.PropertyType == Type) &&
                   (null == Name   || other?.Name == Name);
        }

        #endregion


        #region IResolverPolicy

        public object Resolve<TContext>(ref TContext context)
            where TContext : IBuildContext
        {
            if (Value is IResolverPolicy policy)
                return policy.Resolve(ref context);

            if (Value is IResolverFactory factory)
            {
                var resolveDelegate = factory.GetResolver<TContext>(Type);
                return resolveDelegate(ref context);
            }

            return Value;
        }

        #endregion
    }
}
