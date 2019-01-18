using System;
using Unity.Lifetime;

namespace Unity
{
    /// <summary>
    /// Information about the types registered in a container.
    /// </summary>
    public interface IContainerRegistration
    {
        /// <summary>
        /// Type of the registration.
        /// </summary>
        Type RegisteredType { get; }

        /// <summary>
        /// Name the registered type. Null for default registration.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type that this registration is mapped to. 
        /// </summary>
        Type MappedToType { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        LifetimeManager LifetimeManager { get; }
    }
}
