using System;
using Unity.Lifetime;

namespace Unity.Registration
{
    public class FactoryRegistration : ExplicitRegistration
    {
        #region Constructors

        public FactoryRegistration(UnityContainer owner, Type type, string? name, Func<IUnityContainer, Type, string?, object?> factory, LifetimeManager manager) 
            : base(owner, name, type, manager)
        {
            manager.InUse = true;

            // If Disposable add to container's lifetime
            if (manager is IDisposable managerDisposable) owner.Context.Lifetime.Add(managerDisposable);

            Factory = factory;
            LifetimeManager = manager;
        }

        #endregion

        
        #region Public Properties

        public Func<IUnityContainer, Type, string?, object?> Factory { get; }

        #endregion
    }
}
