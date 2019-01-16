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

        private readonly Type _type;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="ParameterBase"/> that exposes
        /// information about the given <paramref name="parameterType"/>.
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        protected ParameterBase(Type parameterType = null)
        {
            _type = parameterType;
        }


        #endregion


        #region Public Properties

        /// <summary>
        /// The type of parameter this object represents.
        /// </summary>
        public virtual Type ParameterType => _type;

        #endregion


        #region Overrides

        public override bool Equals(Type t)
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

        #endregion
    }
}
