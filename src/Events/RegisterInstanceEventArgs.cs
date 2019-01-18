using System;
using Unity.Extension;
using Unity.Lifetime;

namespace Unity.Events
{
    /// <summary>
    /// Event argument class for the <see cref="ExtensionContext.RegisteringInstance"/> event.
    /// </summary>
    public class RegisterInstanceEventArgs : NamedEventArgs
    {
        /// <summary>
        /// Create a default <see cref="RegisterInstanceEventArgs"/> instance.
        /// </summary>
        public RegisterInstanceEventArgs()
        {
        }

        /// <summary>
        /// Create a <see cref="RegisterInstanceEventArgs"/> instance initialized with the given arguments.
        /// </summary>
        /// <param name="registeredType">Type of instance being registered.</param>
        /// <param name="instance">The instance object itself.</param>
        /// <param name="name">Name to register under, null if default registration.</param>
        /// <param name="lifetimeManager"><see cref="LifetimeManager"/> object that handles how
        /// the instance will be owned.</param>
        public RegisterInstanceEventArgs(Type registeredType, object instance, string name, LifetimeManager lifetimeManager)
            : base(name)
        {
            RegisteredType = registeredType;
            Instance = instance;
            LifetimeManager = lifetimeManager;
        }

        /// <summary>
        /// Type of instance being registered.
        /// </summary>
        /// <value>
        /// Type of instance being registered.
        /// </value>
        public Type RegisteredType { get; }

        /// <summary>
        /// Instance object being registered.
        /// </summary>
        /// <value>Instance object being registered</value>
        public object Instance { get; }

        /// <summary>
        /// <see cref="Unity.LifetimeManager"/> that controls ownership of
        /// this instance.
        /// </summary>
        public LifetimeManager LifetimeManager { get; }
    }
}
