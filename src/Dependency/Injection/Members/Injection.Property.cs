using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unity.Injection
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public sealed class InjectionProperty : InjectionMemberInfo<PropertyInfo>
    {
        #region Constructors

        /// <summary>
        /// Configures the container to inject a specified property with a resolved value.
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        public InjectionProperty(string propertyName)
            : base(propertyName, false)
        {}

        public InjectionProperty(string propertyName, Type contractType, string? contractName)
            : base(propertyName, contractType, contractName, false)
        {
        }

        /// <summary>
        /// Configures the container to inject the given property with the provided value
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        /// <param name="value">Value to be injected into the property.</param>
        public InjectionProperty(string propertyName, object value)
            : base(propertyName, value, false)
        {
        }

        #endregion


        #region Implementation

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected override Type GetMemberType(PropertyInfo member)
            => member.PropertyType;

        #endregion
    }
}
