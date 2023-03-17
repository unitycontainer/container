using System;
using Unity.Import;

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

        private readonly object[] _elementValues;

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
            : base(genericParameterName, Contract.AnyContractName, false)
        {
            _elementValues = elementValues;
        }

        #endregion


        #region IMatch

        protected override MatchRank RankMatch(Type type)
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

        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor) 
            => descriptor.Arguments = _elementValues;

        #endregion
    }
}
