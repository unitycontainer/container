using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
    internal class ContainerLifetimeManager : LifetimeManager
    {
        internal ContainerLifetimeManager(object? data, RegistrationCategory type = RegistrationCategory.Instance)
        {
            Data = data;
            Category = type;
        }

        /// <inheritdoc/>
        public override object? TryGetValue(ICollection<IDisposable> scope) 
            => Data;

        /// <inheritdoc/>
        public override object? GetValue(ICollection<IDisposable> scope) 
            => Data;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
            => throw new NotImplementedException();

        public override ImportSource Source => ImportSource.Local;

        /// <inheritdoc/>
        public override CreationPolicy CreationPolicy => CreationPolicy.Shared;

        /// <inheritdoc/>
        public override string ToString() => "Lifetime: Container";
    }
}
