using System;

namespace Unity.Lifetime
{
    public abstract partial class LifetimeManager
    {
        #region Fields

        internal static ITypeLifetimeManager _typeManager = new TransientLifetimeManager();
        internal static IFactoryLifetimeManager _factoryManager = new TransientLifetimeManager();
        internal static IInstanceLifetimeManager _instanceManager = new ContainerControlledLifetimeManager();

        #endregion


        #region Default Registration Lifetimes

        public static ITypeLifetimeManager DefaultTypeLifetime 
        {
            get => _typeManager;
            set
            { 
                _typeManager = value ?? throw new ArgumentNullException(nameof(DefaultTypeLifetime));
            }
        }

        public static IFactoryLifetimeManager DefaultFactoryLifetime
        {
            get => _factoryManager;
            set
            {
                _factoryManager = value ?? throw new ArgumentNullException(nameof(DefaultFactoryLifetime));
            }
        }

        public static IInstanceLifetimeManager DefaultInstanceLifetime
        {
            get => _instanceManager;
            set
            {
                _instanceManager = value ?? throw new ArgumentNullException(nameof(DefaultInstanceLifetime));
            }
        }

        #endregion
    }
}
