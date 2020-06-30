using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="TransientLifetimeManager"/>,
    /// except container remembers all Disposable objects it created. Once container
    /// is disposed all these objects are disposed as well.
    /// </summary>
    public class ContainerControlledTransientManager : LifetimeManager,
                                                       IFactoryLifetimeManager,
                                                       ITypeLifetimeManager
    {
        public ContainerControlledTransientManager(params InjectionMember[] members)
            : base(members)
        {
        }


        /// <inheritdoc/>
        public override void SetValue(object? newValue, ICollection<IDisposable> lefetime)
        {
            if (newValue is IDisposable disposable)
                lefetime.Add(disposable);
        }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() => this;

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerContainerTransient";
    }
}
