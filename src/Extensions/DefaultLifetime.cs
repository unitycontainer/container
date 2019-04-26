using System;
using Unity.Extension;
using Unity.Lifetime;

namespace Unity
{
    public class DefaultLifetime : UnityContainerExtension
    {
        protected override void Initialize()
        {
        }

        #region Public Members

        public ITypeLifetimeManager TypeDefaultLifetime
        {
            get => (ITypeLifetimeManager)((UnityContainer)Container).TypeLifetimeManager;
            set => ((UnityContainer)Container).TypeLifetimeManager = (LifetimeManager)value ??
                throw new ArgumentNullException("Type Lifetime Manager can not be null");
        }

        public IInstanceLifetimeManager InstanceDefaultLifetime
        {
            get => (IInstanceLifetimeManager)((UnityContainer)Container).InstanceLifetimeManager;
            set => ((UnityContainer)Container).InstanceLifetimeManager = (LifetimeManager)value ??
                throw new ArgumentNullException("Instance Lifetime Manager can not be null");
        }

        public IFactoryLifetimeManager FactoryDefaultLifetime
        {
            get => (IFactoryLifetimeManager)((UnityContainer)Container).FactoryLifetimeManager;
            set => ((UnityContainer)Container).FactoryLifetimeManager = (LifetimeManager)value ??
                throw new ArgumentNullException("Factory Lifetime Manager can not be null");
        }

        #endregion
    }
}
