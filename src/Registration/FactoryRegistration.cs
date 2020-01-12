using System;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity.Registration
{
    public class FactoryRegistration : ExplicitRegistration
    {
        #region Constructors

        public FactoryRegistration(UnityContainer owner, Type type, string? name, Func<IResolveContext, object?> factory, LifetimeManager manager) 
            : base(owner, name, type, manager)
        {
            // If Disposable add to container's lifetime
            if (manager is IDisposable managerDisposable) owner.Context.Lifetime.Add(managerDisposable);

            Factory = factory;
            LifetimeManager = manager;
        }

        #endregion

        
        #region Public Properties

        public Func<IResolveContext, object?> Factory { get; }

        #endregion
    }
}
