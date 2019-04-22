using System;
using System.Diagnostics;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Storage;

namespace Unity.Registration
{
    [DebuggerDisplay("Explicit Registration: MappedTo={Type?.Name ?? string.Empty},    {LifetimeManager?.GetType()?.Name}")]
    public class ExplicitRegistration : ImplicitRegistration
    {
        #region Constructors

        public ExplicitRegistration(Type mappedTo, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers = null)
        {
            Type = mappedTo;
            Key = typeof(LifetimeManager);
            Value = lifetimeManager;
            LifetimeManager.InUse = true;
            InjectionMembers = injectionMembers;
            Next = null;
        }


        public ExplicitRegistration(IPolicySet validators, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers = null)
        {
            Type = null;
            Key = typeof(LifetimeManager);
            Value = lifetimeManager;
            LifetimeManager.InUse = true;
            InjectionMembers = injectionMembers;
            Next = (LinkedNode<Type, object>)validators;
        }

        public ExplicitRegistration(IPolicySet validators, Type mappedTo, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers = null)
        {
            Type = mappedTo;
            Key = typeof(LifetimeManager);
            Value = lifetimeManager;
            LifetimeManager.InUse = true;
            InjectionMembers = injectionMembers;
            Next = (LinkedNode<Type, object>)validators;
        }

        #endregion


        #region IContainerRegistration

        /// <summary>
        /// The type that this registration is mapped to. If no type mapping was done, the
        /// <see cref="Type"/> property and this one will have the same value.
        /// </summary>
        public Type Type { get; }

        #endregion
    }
}
