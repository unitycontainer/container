using System;

namespace Unity.Policy
{
    /// <summary>
    /// Basic information about registered type
    /// </summary>
    public interface INamedType
    {
        /// <summary>
        /// Type of the registration.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Name of the registered type. Null for default registrations.
        /// </summary>
        string Name { get; }
    }
}
