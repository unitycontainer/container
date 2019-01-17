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
        /// <param name="typeFrom">Type to map from.</param>
        /// <param name="typeTo">Type to map to.</param>
        /// <param name="name">Name for the registration.</param>
        /// <param name="lifetimeManager"><see cref="LifetimeManager"/> to manage instances.</param>
        public RegisterEventArgs(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager)
            : base(name)
        {
            TypeFrom = typeFrom;
            TypeTo = typeTo;
            LifetimeManager = lifetimeManager;
        }

        /// <summary>
        /// Type to map from.
        /// </summary>
        public Type TypeFrom { get; }

        /// <summary>
        /// Type to map to.
        /// </summary>
        public Type TypeTo { get; }

        /// <summary>
        /// <see cref="LifetimeManager"/> to manage instances.
        /// </summary>
        public LifetimeManager LifetimeManager { get; }
    }
}
