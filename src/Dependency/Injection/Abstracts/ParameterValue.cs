using System;

namespace Unity.Injection
{
    /// <summary>
    /// Base type for objects that are used to configure parameters for
    /// constructor or method injection, or for getting the value to
    /// be injected into a property.
    /// </summary>
    public abstract class ParameterValue : IEquatable<Type>
    {
        #region IEquatable

        /// <summary>
        /// Checks if this parameter is compatible with the type
        /// </summary>
        /// <param name="type"><see cref="Type"/> to compare to</param>
        /// <returns>True if <see cref="Type"/> is equal</returns>
        public abstract bool Equals(Type type);

        #endregion
    }
}
