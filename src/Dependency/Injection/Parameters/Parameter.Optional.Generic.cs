namespace Unity.Injection
{
    /// <summary>
    /// An injection member that lets you specify that an instance of a 
    /// generic dependency should be resolved, providing the default
    /// value if resolving fails.
    /// </summary>
    public class OptionalGenericParameter : GenericParameterBase
    {
        #region Constructors

        /// <summary>
        /// Create a new instance that specifies that the given generic 
        /// parameter should be optionally resolved from the container.
        /// </summary>
        /// <param name="genericParameterName">Name to the generic parameter</param>
        public OptionalGenericParameter(string genericParameterName)
            : base(genericParameterName, Contract.AnyContractName, true)
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
    }
}
