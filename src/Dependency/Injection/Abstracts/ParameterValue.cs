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

        public abstract bool Equals(Type type);

        #endregion
    }
}
