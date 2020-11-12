using System.Diagnostics;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that lets you specify that
    /// an instance of a generic type parameter should be resolved, providing the <see langword="null"/>
    /// value if resolving fails.
    /// </summary>
    [DebuggerDisplay("OptionalGenericParameter: Type={ParameterTypeName}")]
    public class OptionalGenericParameter : GenericParameterBase
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        public OptionalGenericParameter(string genericParameterName)
            : base(genericParameterName, null, true)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="contractName">Registration name to use when looking up in the container.</param>
        public OptionalGenericParameter(string genericParameterName, string contractName)
            : base(genericParameterName, contractName, true)
        { }

        #endregion


        #region Implementation

        public override string ToString()
        {
            return $"OptionalGenericParameter: Type={ParameterTypeName}";
        }

        #endregion
    }
}
