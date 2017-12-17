using System;

namespace Unity.Registration
{
    /// <summary>
    /// Basic information about registered type
    /// </summary>
    public interface IRegistration
    {
        /// <summary>
        /// Type of the registration.
        /// </summary>
        Type RegisteredType { get; }

        /// <summary>
        /// Name the registered type. Null for default registration.
        /// </summary>
        string Name { get; }
    }
}
