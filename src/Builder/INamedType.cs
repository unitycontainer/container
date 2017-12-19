using System;

namespace Unity.Builder
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
        /// Name the registered type. Null for default registration.
        /// </summary>
        string Name { get; }
    }
}
