using System;


namespace Unity
{
    /// <summary>
    /// Specifies that a property, field, or parameter is an imports a will be
    /// initialized during resolution
    /// </summary>
    public abstract class DependencyResolutionAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="DependencyResolutionAttribute"/> class, 
        ///     importing the dependency with contract name and type.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <param name="type">Contract type</param>
        /// <param name="name">Contract name</param>
        /// <param name="optional">Indicates if dependency is optional</param>
        protected DependencyResolutionAttribute(Type type, string name, bool optional)
        {
            ContractName = name;
            ContractType = type;
            AllowDefault = optional;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DependencyResolutionAttribute"/> class, 
        ///     importing the dependency with new contract type and default name.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <param name="type">Contract type</param>
        /// <param name="optional">Indicates if dependency is optional</param>
        protected DependencyResolutionAttribute(Type type, bool optional)
        {
            ContractType = type;
            AllowDefault = optional;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DependencyResolutionAttribute"/> class, 
        ///     importing the dependency with contract name and default type.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <param name="name">Contract name</param>
        /// <param name="optional">Indicates if dependency is optional</param>
        protected DependencyResolutionAttribute(string name, bool optional)
        {
            ContractName = name;
            AllowDefault = optional;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DependencyResolutionAttribute"/> class, 
        ///     importing the dependency with default contract name and type.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The contract name is compared using a case-sensitive, non-linguistic comparison
        ///         using <see cref="StringComparer.Ordinal"/>.
        ///     </para>
        /// </remarks>
        /// <param name="optional">Indicates if dependency is optional</param>
        protected DependencyResolutionAttribute(bool optional = false)
        {
            AllowDefault = optional;
        }

        #endregion


        #region Public Members

        /// <summary>
        ///     Gets the contract name of the dependency to import.
        /// </summary>
        /// <value>
        ///      A <see cref="string"/> containing the contract name of the dependency to import. The
        ///      default value is an empty string ("").
        /// </value>
        public string? ContractName { get; private set; }

        /// <summary>
        ///     Get the contract type of the dependency to import.
        /// </summary>
        /// <value>
        ///     A <see cref="Type"/> of the dependency that this import is expecting. The default value is
        ///     <see langword="null"/> which means that the type will be obtained by looking at the type on
        ///     the member that this import is attached to. If the type is <see cref="object"/> then the
        ///     importer is declaring they can accept any exported type.
        /// </value>
        public Type? ContractType { get; private set; }


        /// <summary>
        ///     Gets a value indicating whether the property, field or parameter will be set
        ///     to its type's default value when an export with the contract name is not present in
        ///     the container.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the property, field or parameter will be set
        ///     its default value when a dependency can not be resolved; otherwise, <see langword="false"/>.
        ///     The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        ///     <para>
        ///         The default value of a property's, field's or parameter's type is
        ///         <see langword="null"/> for reference types and 0 for numeric value types. For
        ///         other value types, the default value will be each field of the value type
        ///         initialized to zero, if the field is a value type or <see langword="null"/> if
        ///         the field is a reference type.
        ///     </para>
        /// </remarks>
        public bool AllowDefault { get; private set; }

        #endregion
    }
}
