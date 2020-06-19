using System;

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

        public override MatchRank MatchTo(Type type)
        {
            if (null == _type) return MatchRank.NoMatch;

            if (_type.IsGenericTypeDefinition() || type.IsGenericTypeDefinition())
            {
                var left = _type.IsGenericTypeDefinition() ? _type : _type.GetGenericTypeDefinition();
                var right = type.IsGenericTypeDefinition() ?  type : type.GetGenericTypeDefinition();

                return left == right ? MatchRank.ExactMatch : MatchRank.NoMatch;
            }

            return type.IsAssignableFrom(_type)
                ? MatchRank.Compatible
                : MatchRank.NoMatch;
        }

        #endregion


        #region Implementation

        protected bool IsInvalidParameterType
        {
            get
            {
                return null == ParameterType ||
                    ParameterType.IsGenericType() && ParameterType.ContainsGenericParameters() ||
                    ParameterType.IsArray         && ParameterType.GetElementType()!.IsGenericParameter ||
                    ParameterType.IsGenericParameter;
            }
        }


        #endregion
    }
}
