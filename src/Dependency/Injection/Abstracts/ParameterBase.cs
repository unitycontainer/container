using System;
using System.Reflection;
using Unity.Container;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="ParameterValue"/> classes
    /// </summary>
    public abstract class ParameterBase : ParameterValue
    {
        #region Fields

        public readonly Type? ParameterType;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ParameterBase"/> that holds information
        /// about type of import the parameter is injected with
        /// </summary>
        /// <param name="importedType"><see cref="Type"/> to inject</param>
        protected ParameterBase(Type? importedType = null)
        {
            ParameterType = importedType;
        }


        #endregion


        #region Implementation

        public override MatchRank Match(Type type)
        {
            return null == ParameterType 
                ? MatchRank.ExactMatch
                : ParameterType.MatchTo(type);
        }

        public override MatchRank Match(ParameterInfo parameter) => 
            Match(parameter.ParameterType);

        public override InjectionInfo<ParameterInfo> GetInfo(ParameterInfo member)
            => new InjectionInfo<ParameterInfo>(member, ParameterType ?? member.ParameterType, member.HasDefaultValue);

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
