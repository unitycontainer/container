using System;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that lets you specify that
    /// an array containing the registered instances of a generic type parameter 
    /// should be resolved.
    /// </summary>
    public class GenericResolvedArrayParameter : GenericParameterBase
    {
        #region Fields

        private readonly object[] _values;

        #endregion


        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericResolvedArrayParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="elementValues">The values for the elements, that will
        /// be converted to <see cref="ParameterValue"/> objects.</param>
        public GenericResolvedArrayParameter(string genericParameterName, params object[] elementValues)
            : base(genericParameterName, null, false) => _values = elementValues;

        #endregion


        #region IMatch

        protected override MatchRank Match(Type type)
        {
            if (!type.IsArray || type.GetArrayRank() != 1)
                return MatchRank.NoMatch;

            Type elementType = type.GetElementType()!;
            return elementType.Name == base.ParameterTypeName
                ? MatchRank.ExactMatch
                : MatchRank.NoMatch;
        }

        #endregion


        #region Implementation

        /// <summary>
        /// Name for the type represented by this <see cref="ParameterValue"/>.
        /// This may be an actual type name or a generic argument name.
        /// </summary>
        public override string ParameterTypeName => base.ParameterTypeName + "[]";

        public override void GetImportInfo<TImport>(ref TImport import)
        {
            Type type = import.MemberType;

            if (!ReferenceEquals(ContractName, Contract.AnyContractName))
                import.ContractName = ContractName;

            // Optional
            import.AllowDefault |= AllowDefault;

            var (data, resolver) = ResolvedArrayParameter.GetResolver(type, type.GetElementType()!, _values);

            if (null == resolver)
                import.Value = data;
            else
                import.Pipeline = resolver;
        }

        #endregion
    }
}
