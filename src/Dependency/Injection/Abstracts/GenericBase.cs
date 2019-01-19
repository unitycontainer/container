using System;
using System.Reflection;
using Unity.Resolution;

namespace Unity.Injection
{
    /// <summary>
    /// Base class for generic type parameters.
    /// </summary>
    public abstract class GenericBase : ParameterValue,
                                        IResolverFactory<Type>,
                                        IResolverFactory<ParameterInfo>
    {
        #region Fields

        private readonly string _name;
        private readonly bool   _isArray;
        private readonly string _genericParameterName;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        protected GenericBase(string genericParameterName)
            : this(genericParameterName, null)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="resolutionName">Registration name to use when looking up in the container.</param>
        protected GenericBase(string genericParameterName, string resolutionName)
        {
            if (null == genericParameterName) throw new ArgumentNullException(nameof(genericParameterName));

            if (genericParameterName.EndsWith("[]", StringComparison.Ordinal) ||
                genericParameterName.EndsWith("()", StringComparison.Ordinal))
            {
                _genericParameterName = genericParameterName.Replace("[]", string.Empty).Replace("()", string.Empty);
                _isArray = true;
            }
            else
            {
                _genericParameterName = genericParameterName;
                _isArray = false;
            }
            _name = resolutionName;
        }


        #endregion


        #region Public Properties

        /// <summary>
        /// Name for the type represented by this <see cref="ParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public virtual string ParameterTypeName => _genericParameterName;

        #endregion


        #region  Overrides

        public override bool Equals(Type type)
        {
            var t = type ?? throw new ArgumentNullException(nameof(type));
            if (!_isArray)
            {
                return t.GetTypeInfo().IsGenericParameter && t.GetTypeInfo().Name == _genericParameterName;
            }
            return t.IsArray && t.GetElementType().GetTypeInfo().IsGenericParameter && t.GetElementType().GetTypeInfo().Name == _genericParameterName;
        }

        #endregion


        #region IResolverFactory

        public virtual ResolveDelegate<TContext> GetResolver<TContext>(Type type)
            where TContext : IResolveContext
        {
            return GetResolver<TContext>(type, _name);
        }

        public virtual ResolveDelegate<TContext> GetResolver<TContext>(ParameterInfo info)
            where TContext : IResolveContext
        {
            var type = info.ParameterType;
            return GetResolver<TContext>(type, _name);
        }

        #endregion


        #region Implementation

        protected virtual ResolveDelegate<TContext> GetResolver<TContext>(Type type, string name)
            where TContext : IResolveContext
        {
            return (ref TContext context) => context.Resolve(type, name);
        }

        #endregion
    }
}
