using System;
using System.Reflection;

namespace Unity.Injection
{
    /// <summary>
    /// Base type for objects that are used to configure parameters for
    /// constructor or method injection, or for getting the value to
    /// be injected into a property.
    /// </summary>
    public abstract class ParameterValue : IEquatable<Type>,
                                           IEquatable<ParameterInfo>
    {
        #region IEquatable

        /// <summary>
        /// Checks if this parameter is compatible with the type
        /// </summary>
        /// <param name="type"><see cref="Type"/> to compare to</param>
        /// <returns>True if <see cref="Type"/> is equal</returns>
        public abstract bool Equals(Type? type);

        /// <summary>
        /// Checks if this parameter is compatible with the type
        /// </summary>
        /// <param name="other"><see cref="ParameterInfo"/> to compare to</param>
        /// <returns>True if <see cref="ParameterInfo"/> is compatible</returns>
        public abstract bool Equals(ParameterInfo? other);

        #endregion
    }
}
