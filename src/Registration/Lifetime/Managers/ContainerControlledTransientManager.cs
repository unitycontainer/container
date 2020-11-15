using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
        #region Constructors

        public ContainerControlledTransientManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        public override object? TryGetValue(ICollection<IDisposable> scope) 
            => NoValue;

        public override object? GetValue(ICollection<IDisposable> scope)
            => NoValue;

        /// <inheritdoc/>
        public override void SetValue(object? newValue, ICollection<IDisposable> scope)
        {
            if (newValue is IDisposable disposable)
                scope.Add(disposable);
        }

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.EveryTime;

        public override ImportSource Source => ImportSource.Local;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new ContainerControlledTransientManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerContainerTransient";

        #endregion
    }
}
