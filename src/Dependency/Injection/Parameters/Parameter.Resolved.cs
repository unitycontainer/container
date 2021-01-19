using System;
using System.Reflection;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace Unity.Injection
{
    /// <summary>
    /// Instances of this class configure registrations for mandatory injection of
    /// corresponding dependencies with values resolved from the container
    /// </summary>
    /// <remarks>
    /// When the container fails to resolve specified dependency, the 
    /// <see cref="ResolutionFailedException"/> exception is thrown
    /// </remarks>
    [DebuggerDisplay("ResolvedParameter: Type={ParameterType?.Name ?? InferredType} Name={_name}")]
    public class ResolvedParameter : ParameterBase
    {
        #region Fields

        private readonly string? _name;

        #endregion


        #region Constructors

        /// <summary>
        /// Configures the container to 'require' dependency resolved from the container
        /// </summary>
        /// <remarks>
        /// <para>
        /// This constructor does not override either <see cref="Type"/> or name of the import.
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/> or 
        /// <see cref="ImportAttribute"/>, the attribute determines the <see cref="Type"/> and
        /// the Name of the import. Otherwise <see cref="ParameterInfo"/> is used as source of 
        /// <see cref="Type"/>
        /// </para>
        /// <para>
        /// This injection member is useful as a placeholder when registering constructors or methods.
        /// It indicates that parameter in corresponding place should be required, but the <see cref="Type"/>
        /// or the Name of the import <see cref="Contract"/> should not be changed.
        /// </para>
        /// </remarks>
        public ResolvedParameter()
            : base(null, false) 
            => _name = Contract.AnyContractName;

        /// <summary>
        /// Configures the container to 'require' import with specified <see cref="Type"/> and
        /// no name to be resolved from the container
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/> or
        /// <see cref="ImportAttribute"/> and <see cref="Type"/> or Name is specified, both the 
        /// <see cref="Type"/> and the Name are ignored.
        /// </remarks>
        /// <param name="contractType">Type of resolved dependency</param>
        public ResolvedParameter(Type contractType)
            : base(contractType, false) 
            => _name = null;
        
        /// <summary>
        /// Configures the container to 'require' dependency to be resolved with specific 
        /// contract name.
        /// </summary>
        /// <remarks>
        /// The <see cref="Type"/> of imported contract is not affected and name of the 
        /// <see cref="Contract"/> is changed to be <paramref name="contractName"/>
        /// </remarks>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public ResolvedParameter(string contractName)
            : base(null, false) 
            => _name = contractName;

        /// <summary>
        /// Configures the container to 'require' import to be resolved with specified 
        /// <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// If the parameter is annotated with <see cref="DependencyResolutionAttribute"/>, 
        /// the attribute is ignored.
        /// </remarks>
        /// <param name="contractType">Type of the <see cref="Contract"/></param>
        /// <param name="contractName">Name of the <see cref="Contract"/></param>
        public ResolvedParameter(Type contractType, string? contractName)
            : base(contractType, false) 
            => _name = contractName;

        #endregion


        #region Implementation

        public override void ProvideImport<TContext, TDescriptor>(ref TDescriptor descriptor)
        {
            if (!ReferenceEquals(_name, Contract.AnyContractName))
                descriptor.ContractName = _name;
            
            if (ParameterType is not null && !ParameterType.IsGenericTypeDefinition)
                descriptor.ContractType = ParameterType;

            descriptor.AllowDefault = AllowDefault;
        }

        public override string ToString() 
            => $"ResolvedParameter: Type={ParameterType?.Name ?? InferredType} Name={_name ?? "null"}";

        #endregion
    }


    #region Generic

    /// <summary>
    /// A generic version of <see cref="ResolvedParameter"/>
    /// </summary>
    /// <typeparam name="T">The <see cref="Type"/> of injected <see cref="Contract"/></typeparam>
    public class ResolvedParameter<T> : ResolvedParameter
    {
        /// <inheritdoc/>
        public ResolvedParameter() : base(typeof(T))
        {
        }

        /// <inheritdoc/>
        public ResolvedParameter(string name) : base(typeof(T), name)
        {
        }
    }

    #endregion
}