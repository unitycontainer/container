using System;
using Unity.Container;

namespace Unity.Extension
{
    public interface IImportIMembernfo//<TMemberInfo>
    {
        /// <summary>
        /// Type of resolved member
        /// </summary>
        /// <remarks>
        /// This is the type of the resolved member. If resolving a Property, 
        /// this is <see cref="PropertyInfo.PropertyType"/> of that property. 
        /// If member is a parameter, this is <see cref="ParameterInfo.ParameterType"/>, 
        /// and etc.
        /// </remarks>
        Type MemberType { get; }

        /// <summary>
        /// Declaring <see cref="Type"/> of resolved member
        /// </summary>
        Type DeclaringType { get; }

        /// <summary>
        /// Type of resolved <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// Injection member can override <see cref="Contract"/> <see cref="Type"/> with 
        /// any other assignable <see cref="Type"/>.
        /// </remarks>
        Type ContractType { get; }

        /// <summary>
        /// Name of the <see cref="Contract"/>
        /// </summary>
        /// <remarks>
        /// <see cref="Contract"/> name could be overridden with new name
        /// </remarks>
        string? ContractName { get; }

        Attribute[]? Attributes { get; }
    }
}
