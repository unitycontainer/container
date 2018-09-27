

using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="InjectionParameterValue"/> classes
    /// that deal in explicit types.
    /// </summary>
    public abstract class TypedInjectionValue : InjectionParameterValue
    {
        private readonly Type _type;

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

        /// <summary>
        /// The type of parameter this object represents.
        /// </summary>
        public virtual Type ParameterType => _type;

        /// <summary>
        /// Name for the type represented by this <see cref="InjectionParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName => _type.GetTypeInfo().Name;

        /// <summary>
        /// Test to see if this parameter value has a matching type for the given type.
        /// </summary>
        /// <param name="t">Type to check.</param>
        /// <returns>True if this parameter value is compatible with type <paramref name="t"/>,
        /// false if not.</returns>
        public override bool MatchesType(Type t)
        {
            var cInfo = (t ?? throw new ArgumentNullException(nameof(t))).GetTypeInfo();
            var info = _type.GetTypeInfo();

            if (cInfo.IsGenericType && cInfo.ContainsGenericParameters && info.IsGenericType && info.ContainsGenericParameters)
            {
                return t.GetGenericTypeDefinition() ==  _type.GetGenericTypeDefinition();
            }

            return cInfo.IsAssignableFrom(info);
        }
    }
}
