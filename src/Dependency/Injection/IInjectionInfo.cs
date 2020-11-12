using System;
using Unity.Container;
using Unity.Resolution;

namespace Unity.Injection
{
    public interface IInjectionInfo : IImportInfo
    {
        /// <summary>
        /// Type of resolved <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// Injection member can override <see cref="Contract"/> <see cref="Type"/> with 
        /// any other assignable <see cref="Type"/>.
        /// </remarks>
        new Type ContractType { get; set; }

        /// <summary>
        /// Name of the <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// <see cref="Contract"/> name could be overridden with new name
        /// </remarks>
        new string? ContractName { get; set; }

        /// <summary>
        /// Determines if default value is allowed
        /// </summary>
        /// <remarks>
        /// This value will tell the container if resolved dependency is required or
        /// optional. 
        /// Whether import is optional or required determined by several factors. If,
        /// for example, a parameter has default value, it will always be optional.
        /// </remarks>
        bool AllowDefault { get; set; }

        /// <summary>
        /// Set injected value
        /// </summary>
        /// <remarks>
        /// A value set by this setter will be injected as is, with no additional
        /// processing.
        /// </remarks>
        object? Value { set; }
        
        /// <summary>
        /// Set external (unknown) value
        /// </summary>
        /// <remarks>
        /// This setter should be used when injection member does not know exact 
        /// nature of provided value. For example, it could be a value, a resolver,
        /// another <see cref="InjectionMember"/>, and etc.
        /// </remarks>
        object? External { set; }
        
        /// <summary>
        /// Set pipeline
        /// </summary>
        /// <remarks>
        /// This setter allows injection of resolution pipeline that could 
        /// be used to create injected value
        /// </remarks>
        ResolveDelegate<PipelineContext> Pipeline { set; }
    }
}
