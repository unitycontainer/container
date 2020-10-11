using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// This class stores information about which properties to inject,
    /// and will configure the container accordingly.
    /// </summary>
    public class OptionalProperty : InjectionMemberInfo<PropertyInfo>
    {
        #region Constructors

        /// <summary>
        /// Configures the container to inject a specified property with a resolved value.
        /// </summary>
        /// <param name="propertyName">Name of property to inject.</param>
        public OptionalProperty(string propertyName)
            : base(propertyName)
        {
        }

        public OptionalProperty(string propertyName, Type contractType)
            : base(propertyName, contractType)
        {

        }

        public OptionalProperty(string propertyName, Type contractType, string? contractName)
            : base(propertyName, contractType, contractName)
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
            
        // TODO: Add generic cases

        #endregion


        #region Implementation

        public override object? Resolve<TContext>(ref TContext context)
        {
            return base.Resolve(ref context);
        }

        #endregion
    }
}
