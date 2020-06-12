using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="ParameterValue"/> classes
    /// that deal in explicit types.
    /// </summary>
    public abstract class ParameterBase : ParameterValue
    {
        #region Fields

        private readonly Type? _type;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="ParameterBase"/> that exposes
        /// information about the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of the parameter.</param>
        protected ParameterBase(Type? type = null)
        {
            _type = type;
        }


        #endregion


        #region Public Properties

        /// <summary>
        /// The type of parameter this object represents.
        /// </summary>
        public virtual Type? ParameterType => _type;

        #endregion


        #region Overrides

        public override bool Equals(Type? type)
        {
            if (null == _type) return true;
            if (null == type) return false;

#if NETSTANDARD1_0 || NETCOREAPP1_0
            var info = _type.GetTypeInfo();
            var cInfo = type.GetTypeInfo();
#else
            var info = _type;
            var cInfo = type;
#endif
            if (info.IsGenericTypeDefinition || cInfo.IsGenericTypeDefinition)
            {
                var left = info.IsGenericTypeDefinition  ? _type : _type.GetGenericTypeDefinition();
                var right = cInfo.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition();

                return left == right;
            }

            return cInfo.IsAssignableFrom(info);
        }

        public override bool Equals(ParameterInfo? other)
        {
            if (null == other) return false;

            return Equals(other.ParameterType);
        }

        #endregion


        #region Implementation

        protected bool IsInvalidParameterType
        {
            get
            {
#if NETSTANDARD1_0 || NETCOREAPP1_0
                var info = ParameterType?.GetTypeInfo();
                return null == ParameterType || null == info || 
                    info.IsGenericType && info.ContainsGenericParameters ||
                    ParameterType.IsArray && ParameterType.GetElementType().GetTypeInfo().IsGenericParameter ||
                    ParameterType.IsGenericParameter;
#else
                return null == ParameterType ||
                    ParameterType.IsGenericType && ParameterType.ContainsGenericParameters ||
                    ParameterType.IsArray && ParameterType.GetElementType()!.IsGenericParameter ||
                    ParameterType.IsGenericParameter;
#endif
            }
        }


        #endregion
    }
}
