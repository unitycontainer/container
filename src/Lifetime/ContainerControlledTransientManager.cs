using System;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="TransienLifetimeManager"/>,
    /// except container remembers all Disposable objects it created. Once container
    /// is disposed all these objects are disposed as well.
    /// </summary>
    public class ContainerControlledTransientManager : LifetimeManager
    {
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            if (newValue is IDisposable disposable)
                container?.Add(disposable);
        }

        public override object GetValue(ILifetimeContainer container = null)
        {
            return null;
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return this;
        }

        public override bool InUse { get => false; set => base.InUse = false; }
    }
}
