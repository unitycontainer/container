using System;
using System.Diagnostics;

namespace Unity.Injection
{
    /// <summary>
    /// Instances of this class configure registrations for optional injection of
    /// corresponding dependencies with values resolved from the container
    /// </summary>
    /// <remarks>
    /// When the container fails to resolve requested dependency, the error is not generated. 
    /// The dependency is either injected with default value, or 'default(T)'
    /// </remarks>
    [DebuggerDisplay("OptionalParameter: Type={ParameterType?.Name ?? InferredType} Name={_name}")]
    public class OptionalParameter : ParameterBase
    {
        #region Fields

        private readonly string? _name;

        #endregion


        #region Constructors

        /// <summary>
        /// Configures the container to inject dependency with optional value resolved 
        /// from the container
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <see cref="Type"/> and the Name of imported contract is not affected by this
        /// injection member. 
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>or 
        /// <see cref="ImportAttribute"/>, the attribute is used for <see cref="Contract"/> info.
        /// </para>
        /// <para>
        /// This injection member is useful as a placeholder when registering constructors or methods.
        /// It indicates that parameter in corresponding place should be imported optionally, 
        /// and the <see cref="Type"/> or the Name of imported <see cref="Contract"/> should not be changed.
        /// </para>
        /// </remarks>
        public OptionalParameter()
            : base(null, true) 
            => _name = Contract.AnyContractName;

        /// <summary>
        /// Configures the container to import dependency with optionally resolved contract
        /// with specified <see cref="Type"/> and no name from the container
        /// <see cref="Type"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractType">Type of this parameter.</param>
        public OptionalParameter(Type contractType)
            : base(contractType, true) 
            => _name = null;

        /// <summary>
        /// Configures the container to optionally inject dependency with <see cref="Contract"/> 
        /// with specified name.
        /// </summary>
        /// <remarks>
        /// The parameter is injected with value imported from the container. The <see cref="Type"/> of 
        /// imported contract is the <see cref="Type"/> of the parameter and name of the 
        /// <see cref="Contract"/> is provided in <paramref name="contractName"/>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public OptionalParameter(string contractName)
            : base(null, true) => 
            _name = contractName;

        /// <summary>
        /// Configures the container to optionally inject dependency with specified <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public OptionalParameter(Type contractType, string contractName)
            : base(contractType, true) 
            => _name = contractName;

        #endregion


        #region Implementation

        public override void DescribeImport<TDescriptor>(ref TDescriptor descriptor)
        {
            if (!ReferenceEquals(_name, Contract.AnyContractName))
                descriptor.ContractName = _name;

            base.DescribeImport(ref descriptor);
        }

        public override string ToString() 
            => $"OptionalParameter: Type={ParameterType?.Name ?? InferredType} Name={_name ?? "null"}";

        #endregion
    }


    #region Generic

    /// <summary>
    /// A generic version of <see cref="OptionalParameter"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of injected <see cref="Contract"/></typeparam>
    public class OptionalParameter<T> : OptionalParameter
    {
        /// <inheritdoc/>
        public OptionalParameter() : base(typeof(T))
        {
        }

        /// <inheritdoc/>
        public OptionalParameter(string name) : base(typeof(T), name)
        {
        }
    }

    #endregion
}
