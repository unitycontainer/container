using System;
using System.ComponentModel.Composition;
using System.Reflection;

namespace Unity.Extension
{
    public interface IImportMemberDescriptor
    {
        #region Member

        /// <summary>
        /// <see cref="Type"/> of imported member, set by <see cref="MemberInfo"/>
        /// </summary>
        Type MemberType { get; }

        /// <summary>
        /// Declaring <see cref="Type"/>, set by <see cref="MemberInfo"/>
        /// </summary>
        Type DeclaringType { get; }

        #endregion


        #region Contract

        Type ContractType { get; set; }
        
        string? ContractName { get; set; }
        
        #endregion


        #region Metadata

        /// <summary>
        /// True if annotated with <see cref="ImportAttribute"/> or 
        /// <see cref="ImportManyAttribute"/>
        /// </summary>
        bool IsImport { get; set; }

        /// <summary>
        /// Determines where import is resolved from
        /// </summary>
        ImportSource Source { get; set; }

        /// <summary>
        /// Creation policy of the import
        /// </summary>
        CreationPolicy Policy { get; set; }

        #endregion


        #region Default Value

        /// <summary>
        /// Allows default value if can not be resolved
        /// </summary>
        bool AllowDefault { get; set; }

        /// <summary>
        /// Sets default value and flips <see cref="AllowDefault"/> to <see cref="True"/>
        /// </summary>
        object? Default { set; }

        #endregion


        #region Value Setters

        /// <summary>
        /// Set import value
        /// </summary>
        /// <remarks>
        /// A value set by this setter will be injected as is, with no additional processing.
        /// </remarks>
        object? Value { set; }

        /// <summary>
        /// Set dynamic (unknown) value
        /// </summary>
        /// <remarks>
        /// This setter used when runtime value is not known at design time. The type of the value
        /// will be analyzed at runtime. The value could be a resolver, another <see cref="InjectionMember"/>, 
        /// and etc.
        /// </remarks>
        object? Dynamic { set; }

        /// <summary>
        /// Set resolution pipeline
        /// </summary>
        Delegate Pipeline { set; }

        /// <summary>
        /// Indicates that no data is injected
        /// </summary>
        void None();

        #endregion
    }

    public interface IImportMemberDescriptor<TMemberInfo> : IImportMemberDescriptor
    {
        /// <summary>
        /// One of <see cref="ParameterInfo"/>, <see cref="FieldInfo"/>, or
        /// <see cref=" PropertyInfo"/>
        /// </summary>
        TMemberInfo MemberInfo { get; }
    }
}
