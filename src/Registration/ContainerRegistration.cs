using System;
using System.Diagnostics;
using Unity.Injection;

namespace Unity.Registration
{
    [DebuggerDisplay("ContainerRegistration: Type={RegisteredType?.Name},    Name={Name},    MappedTo={RegisteredType == MappedToType ? string.Empty : MappedToType?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
    public class ContainerRegistration : InternalRegistration
    {
        #region Constructors

        public ContainerRegistration(Type mappedTo, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers = null)
            : base()
        {
            Type = mappedTo;
            Key = typeof(LifetimeManager);
            Value = lifetimeManager;
            LifetimeManager.InUse = true;
            InjectionMembers = injectionMembers;
        }

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="InternalRegistration.Type"/> property and this one will have the same value.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// The lifetime manager for this registration.
        /// </summary>
        /// <remarks>
        /// This property will be null if this registration is for an open generic.</remarks>
        public LifetimeManager LifetimeManager => (LifetimeManager)Value;

        #endregion
    }
}
