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
    public class TypeBasedOverride : ResolverOverride
    {
        private readonly Type _targetType;
        private readonly ResolverOverride _innerOverride;

        /// <summary>
        /// Create an instance of <see cref="TypeBasedOverride"/>
        /// </summary>
        /// <param name="targetType">Type to check for.</param>
        /// <param name="innerOverride">Inner override to check after type matches.</param>
        public TypeBasedOverride(Type targetType, ResolverOverride innerOverride)
        {
            _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            _innerOverride = innerOverride ?? throw new ArgumentNullException(nameof(innerOverride));
        }

        /// <summary>
        /// Return a <see cref="IResolverPolicy"/> that can be used to give a value
        /// for the given desired dependency.
        /// </summary>
        /// <param name="context">Current build context.</param>
        /// <param name="dependencyType">Type of dependency desired.</param>
        /// <returns>a <see cref="IResolverPolicy"/> object if this override applies, null if not.</returns>
        public override IResolverPolicy GetResolver<TBuilderContext>(ref TBuilderContext context, Type dependencyType)
        {
            switch (context.CurrentOperation)
            {
                case PropertyInfo property when property.DeclaringType == _targetType:
                    return _innerOverride.GetResolver(ref context, dependencyType);

                case ParameterInfo parameter when parameter.Member.DeclaringType == _targetType:
                    return _innerOverride.GetResolver(ref context, dependencyType);

                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// A convenience version of <see cref="TypeBasedOverride"/> that lets you
    /// specify the type to construct via generics syntax.
    /// </summary>
    /// <typeparam name="T">Type to check for.</typeparam>
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
