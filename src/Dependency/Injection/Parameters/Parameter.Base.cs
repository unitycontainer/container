using System;

namespace Unity.Injection
{
    /// <summary>
    /// A base class for implementing <see cref="ParameterValue"/> classes
    /// </summary>
    public abstract class ParameterBase : ParameterValue
    {
        #region Fields

        protected const string  InferredType = "Inferred At Runtime";
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

        protected override MatchRank Match(Type type)
        {
            return ParameterType is null
                ? MatchRank.ExactMatch
                : ParameterType.MatchTo(type);
        }

        #endregion


        #region ImportInfo

        public override void GetImportInfo<TImport>(ref TImport import)
        {
            if (ParameterType is not null && !ParameterType.IsGenericTypeDefinition)
                import.ContractType = ParameterType;

            import.AllowDefault = AllowDefault;
        }

        #endregion
    }
}
