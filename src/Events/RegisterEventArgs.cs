using System;
using Unity.Extension;
using Unity.Lifetime;

namespace Unity.Events
{
    /// <summary>
    /// Event argument class for the <see cref="ExtensionContext.Registering"/> event.
    /// </summary>
    public class RegisterEventArgs : NamedEventArgs
    {
        /// <summary>
        /// Create a new instance of <see cref="RegisterEventArgs"/>.
        /// </summary>
        /// <param name="registeredType">Type to map from.</param>
        /// <param name="mappedTo">Type to map to.</param>
        /// <param name="name">Name for the registration.</param>
        /// <param name="lifetimeManager"><see cref="LifetimeManager"/> to manage instances.</param>
        public RegisterEventArgs(Type registeredType, Type? mappedTo, string? name, LifetimeManager lifetimeManager)
            : base(name)
        {
            TypeFrom = registeredType;
            TypeTo = mappedTo;
            LifetimeManager = lifetimeManager;
        }

        /// <summary>
        /// Type to map from.
        /// </summary>
        public Type TypeFrom { get; }

        /// <summary>
        /// Type to map to.
        /// </summary>
        public Type? TypeTo { get; }

        /// <summary>
        /// <see cref="LifetimeManager"/> to manage instances.
        /// </summary>
        public LifetimeManager LifetimeManager { get; }
    }
}
