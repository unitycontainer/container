using System;
using System.Diagnostics;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity.Registration
{
    [DebuggerDisplay("ContainerRegistration: MappedTo={Type?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
    public class ContainerRegistration : InternalRegistration
    {
        #region Constructors

        public ContainerRegistration(LinkedNode<Type, object> validators, Type mappedTo, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers = null)
        {
            Type = mappedTo;
            Key = typeof(LifetimeManager);
            Value = lifetimeManager;
            LifetimeManager.InUse = true;
            InjectionMembers = injectionMembers;
            Next = validators;
        }

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="Type"/> property and this one will have the same value.
        /// </summary>
        public virtual Type Type { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public virtual LifetimeManager LifetimeManager => (LifetimeManager)Value;

        #endregion
    }
}
