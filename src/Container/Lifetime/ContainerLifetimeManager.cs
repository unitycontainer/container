using System;
using System.Collections.Generic;
using Unity.Lifetime;

namespace Unity.Container
{
    /// <summary>
    /// Internal container lifetime manager. 
    /// This manager distinguishes internal registration from user mode registration.
    /// </summary>
    /// <remarks>
    /// Works like the ExternallyControlledLifetimeManager, but uses 
    /// regular instead of weak references
    /// </remarks>
    public class ContainerLifetimeManager : LifetimeManager
    {
        internal ContainerLifetimeManager(object? data, RegistrationCategory type = RegistrationCategory.Instance)
        {
            Data = data;
            Category = type;
        }

        public override object? GetValue(ICollection<IDisposable> lifetime) => Data;

        protected override LifetimeManager OnCreateLifetimeManager()
            => throw new NotImplementedException();

        public override string ToString() => "Lifetime: Container";
    }
}
