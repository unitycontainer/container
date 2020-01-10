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

        public ContainerRegistration(UnityContainer container, LinkedNode<Type?, object?>? validators, Type mappedTo, LifetimeManager lifetimeManager, InjectionMember[]? injectionMembers = null)
            : base(container)
        {
            Type = mappedTo;
            Key = typeof(LifetimeManager);
            Value = lifetimeManager;
            InjectionMembers = null != injectionMembers && 0 == injectionMembers.Length ? null : injectionMembers;
            Next = validators;
        }

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="Type"/> property and this one will have the same value.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager => (LifetimeManager)Value!;

        #endregion
    }
}
