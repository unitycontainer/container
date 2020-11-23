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

        protected readonly bool  AllowDefault;
        protected readonly Type? ParameterType;

        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ParameterBase"/> that holds information
        /// about type of import the parameter is injected with
        /// </summary>
        /// <param name="importedType"><see cref="Type"/> to inject</param>
        protected ParameterBase(Type? importedType, bool optional)
        {
            AllowDefault  = optional;
            ParameterType = importedType;
        }


        #endregion


        #region Match

        public override MatchRank Match(Type type)
        {
            // TODO: Cases in between
            return null == ParameterType 
                ? MatchRank.ExactMatch
                : ParameterType.MatchTo(type);
        }

        public override MatchRank Match(ParameterInfo parameter) 
            => Match(parameter.ParameterType);

        #endregion


        #region ImportInfo

        public override void GetImportInfo<TImport>(ref TImport import)
        {
            if (null != ParameterType && !ParameterType.IsGenericTypeDefinition)
                import.ContractType = ParameterType;

            import.AllowDefault = AllowDefault;
        }

        #endregion
    }
}
