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
            get => ((UnityContainer)Container).Defaults.TypeLifetimeManager;
            set => ((UnityContainer)Container).Defaults.TypeLifetimeManager = value ??
                throw new ArgumentNullException("Type Lifetime Manager can not be null");
        }

        public IInstanceLifetimeManager InstanceDefaultLifetime
        {
            get => ((UnityContainer)Container).Defaults.InstanceLifetimeManager;
            set => ((UnityContainer)Container).Defaults.InstanceLifetimeManager = value ??
                throw new ArgumentNullException("Instance Lifetime Manager can not be null");
        }

        public IFactoryLifetimeManager FactoryDefaultLifetime
        {
            get => ((UnityContainer)Container).Defaults.FactoryLifetimeManager;
            set => ((UnityContainer)Container).Defaults.FactoryLifetimeManager = value ??
                throw new ArgumentNullException("Factory Lifetime Manager can not be null");
        }

        #endregion
    }
}
