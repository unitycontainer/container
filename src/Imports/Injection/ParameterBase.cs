using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="ParameterValue"/> classes
    /// </summary>
    public abstract class ParameterBase : ParameterValue
    {
        #region Fields

        private readonly Type? _type;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ParameterBase"/> that holds information
        /// about type of import the parameter is injected with
        /// </summary>
        /// <param name="importedType"><see cref="Type"/> to inject</param>
        protected ParameterBase(Type? importedType = null)
        {
            _type = importedType;
        }


        #endregion


        #region Public Properties

        /// <summary>
        /// The type of parameter this object represents.
        /// </summary>
        public virtual Type? ParameterType => _type;

        #endregion


        #region Overrides

        public override MatchRank Match(Type type)
        {
            return null == _type 
                ? MatchRank.ExactMatch
                : _type.MatchTo(type);
        }

        public override MatchRank Match(ParameterInfo parameter) => 
            Match(parameter.ParameterType);

        #endregion


        #region Implementation

        protected bool IsInvalidParameterType
        {
            get
            {
                return null == ParameterType ||
                    ParameterType.IsGenericType && ParameterType.ContainsGenericParameters ||
                    ParameterType.IsArray       && ParameterType.GetElementType()!.IsGenericParameter ||
                    ParameterType.IsGenericParameter;
            }
        }


        #endregion
    }
}
