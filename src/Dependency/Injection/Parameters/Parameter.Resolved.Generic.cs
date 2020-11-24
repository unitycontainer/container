using System.Diagnostics;

namespace Unity.Injection
{
    /// <summary>
    /// A <see cref="ParameterValue"/> that lets you specify that
    /// an instance of a generic type parameter should be resolved.
    /// </summary>
    [DebuggerDisplay("GenericParameter: Type={ParameterTypeName}")]
    public class GenericParameter : GenericParameterBase
    {
        #region Constructors

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        public GenericParameter(string genericParameterName)
            : base(genericParameterName, null, false)
        { }

        /// <summary>
        /// Create a new <see cref="GenericParameter"/> instance that specifies
        /// that the given named generic parameter should be resolved.
        /// </summary>
        /// <param name="genericParameterName">The generic parameter name to resolve.</param>
        /// <param name="contractName">Registration name to use when looking up in the container.</param>
        public GenericParameter(string genericParameterName, string contractName)
            : base(genericParameterName, contractName, false)
        { }

        #endregion


        #region Overrides

        public override string ToString() 
            => $"GenericParameter: Type={ParameterTypeName}";

        #endregion
    }
}
