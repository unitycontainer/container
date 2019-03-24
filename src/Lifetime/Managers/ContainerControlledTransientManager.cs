using System;

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
        /// <inheritdoc/>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            if (newValue is IDisposable disposable)
                container?.Add(disposable);
        }

        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            return null;
        }

        /// <inheritdoc/>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
        }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return this;
        }

        /// <inheritdoc/>
        public override bool InUse { get => false; set => base.InUse = false; }


        #region Overrides

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerContainerTransient";

        #endregion
    }
}
