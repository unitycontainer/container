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
        #region Constructors

        public ContainerControlledTransientManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        public override object? TryGetValue(ICollection<IDisposable> lifetime) 
            => NoValue;

        public override object? GetValue(ICollection<IDisposable> lifetime)
            => NoValue;

        /// <inheritdoc/>
        public override void SetValue(object? newValue, ICollection<IDisposable> lefetime)
        {
            if (newValue is IDisposable disposable)
                lefetime.Add(disposable);
        }

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.EveryTime;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new ContainerControlledTransientManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerContainerTransient";

        #endregion
    }
}
