using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="InjectionParameterValue"/> classes
    /// that deal in explicit types.
    /// </summary>
    public abstract class TypedInjectionValue : InjectionParameterValue,
                                                IEquatable<Type>
    {
        #region Fields

        private readonly Type _type;

        #endregion


        #region Constructors

        public TypedInjectionValue()
        {

        }

        /// <summary>
        /// Create a new <see cref="TypedInjectionValue"/> that exposes
        /// information about the given <paramref name="parameterType"/>.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        protected TypedInjectionValue(Type parameterType, object value)
            : base(value)
        {
            _type = parameterType;
        }


        #endregion

        /// <summary>
        /// The type of parameter this object represents.
        /// </summary>
        public virtual Type ParameterType => _type;

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName => _type.GetTypeInfo().Name;

        public bool Equals(Type t)
        {
            if (null == _type) return true;

            var cInfo = (t ?? throw new ArgumentNullException(nameof(t))).GetTypeInfo();
            var info = _type.GetTypeInfo();

            if (cInfo.IsGenericType && cInfo.ContainsGenericParameters && info.IsGenericType && info.ContainsGenericParameters)
            {
                return t.GetGenericTypeDefinition() == _type.GetGenericTypeDefinition();
            }

            return cInfo.IsAssignableFrom(info);
        }
    }
}
