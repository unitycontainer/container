using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public sealed class OptionalProperty : InjectionMemberInfo<PropertyInfo>
    {
        #region Constructors

        /// <summary>
        /// Configures the container to inject a specified property with a resolved value.
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        public OptionalProperty(string propertyName)
            : base(propertyName, true)
        {
        }

        public OptionalProperty(string propertyName, Type contractType)
            : base(propertyName, contractType, true)
        {

        }

        public OptionalProperty(string propertyName, string? contractName)
            : base(propertyName, contractName, true)
        {

        }

        public OptionalProperty(string propertyName, Type contractType, string? contractName)
            : base(propertyName, contractType, contractName, true)
        {

        }

        /// <summary>
        /// Configures the container to inject the given property with the provided value
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        /// <param name="value">Value to be injected into the property.</param>
        public OptionalProperty(string propertyName, object value)
            : base(propertyName, value)
        {
        }

        #endregion
    }
}
