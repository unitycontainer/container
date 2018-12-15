using System;
using System.Reflection;
using Unity.Policy;

namespace Unity.Resolution
{
    /// <summary>
    /// An implementation of <see cref="ResolverOverride"/> that
    /// acts as a decorator over another <see cref="ResolverOverride"/>.
    /// This checks to see if the current type being built is the
    /// right one before checking the inner <see cref="ResolverOverride"/>.
    /// </summary>
    [Obsolete("This type has been deprecated as degrading performance. Use DependencyOverride instead.", false)]
    public class TypeBasedOverride : ResolverOverride,
                                     IEquatable<ParameterInfo>,
                                     IEquatable<PropertyInfo>
    {
        #region Fields

        private readonly ResolverOverride _innerOverride;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an instance of <see cref="TypeBasedOverride"/>
        /// </summary>
        /// <param name="targetType">Type to check for.</param>
        /// <param name="innerOverride">Inner override to check after type matches.</param>
        public TypeBasedOverride(Type targetType, ResolverOverride innerOverride)
            : base(targetType, null, null)
        {
            _innerOverride = (innerOverride ?? throw new ArgumentNullException(nameof(innerOverride)))
                .OnType(targetType ?? throw new ArgumentNullException(nameof(targetType)));
        }

        #endregion


        #region ResolverOverride

        public override ResolveDelegate<TContext> GetResolver<TContext>(Type type)
        {
            return _innerOverride.GetResolver<TContext>(type);
        }

        #endregion


        #region IEquatable

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return _innerOverride.Equals(obj);
        }

        public bool Equals(PropertyInfo other)
        {
            return _innerOverride is IEquatable<PropertyInfo> info && 
                   info.Equals(other);
        }

        public bool Equals(ParameterInfo other)
        {
            return _innerOverride is IEquatable<ParameterInfo> info && 
                   info.Equals(other);
        }

        #endregion
    }

    /// <summary>
    /// A convenience version of <see cref="TypeBasedOverride"/> that lets you
    /// specify the type to construct via generics syntax.
    /// </summary>
    /// <typeparam name="T">Type to check for.</typeparam>
    [Obsolete("This type has been deprecated as degrading performance. Use DependencyOverride instead.", false)]
    public class TypeBasedOverride<T> : TypeBasedOverride
    {
        /// <summary>
        /// Create an instance of <see cref="TypeBasedOverride{T}"/>.
        /// </summary>
        /// <param name="innerOverride">Inner override to check after type matches.</param>
        public TypeBasedOverride(ResolverOverride innerOverride)
            : base(typeof(T), innerOverride)
        {
        }
    }
}
