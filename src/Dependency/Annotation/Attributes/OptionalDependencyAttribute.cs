using System;

namespace Unity
{
    /// <summary>
    /// This attribute is used to mark properties, fields and parameters as 
    /// targets for optional injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OptionalDependencyAttribute : DependencyResolutionAttribute
    {
        #region Constructors

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object.
        /// </summary>
        public OptionalDependencyAttribute()
            : base(true) 
        {
        }

        /// <summary>
        /// Construct a new <see cref="OptionalDependencyAttribute"/> object that
        /// specifies a named dependency.
        /// </summary>
        /// <param name="name">Name of the dependency.</param>
        public OptionalDependencyAttribute(string name)
            : base(name, true) 
        {
        }


        public OptionalDependencyAttribute(Type type)
            : base(type, true)
        {
        }

        public OptionalDependencyAttribute(Type type, string name)
            : base(type, name, true)
        {
        }

        #endregion


        #region Public Members

        /// <summary>
        /// The name specified in the constructor.
        /// </summary>
        public string? Name => ContractName;

        #endregion
    }
}
