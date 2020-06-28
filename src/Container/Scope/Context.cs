using System;
using Unity.Extension;
using Unity.Lifetime;

namespace Unity.Scope
{
    public partial class RegistrationScope : ContainerContext
    {
        #region Fields

        private LifetimeManager _typeLifetime;
        private LifetimeManager _factoryLifetime;
        private LifetimeManager _instanceLifetime;

        #endregion


        public override ILifetimeContainer LifetimeContainer { get; }


        #region Default Lifetime Managers

        public override LifetimeManager DefaultTypeLifetime
        {
            get => _typeLifetime;
            set
            {
                if (!(value is ITypeLifetimeManager)) 
                    throw new ArgumentException("Default Type LifetimeManager must implement ITypeLifetimeManager");
                
                _typeLifetime = value;
            }
        }

        public override LifetimeManager DefaultFactoryLifetime
        { 
            get => _factoryLifetime;
            set
            {
                if (!(value is IFactoryLifetimeManager))
                    throw new ArgumentException("Default Factory LifetimeManager must implement IFactoryLifetimeManager");

                _factoryLifetime = value;
            }
        }

        public override LifetimeManager DefaultInstanceLifetime 
        { 
            get => _instanceLifetime;
            set
            {
                if (!(value is IInstanceLifetimeManager))
                    throw new ArgumentException("Default Type LifetimeManager must implement IInstanceLifetimeManager");

                _instanceLifetime = value;
            }
        }

        #endregion
    }
}
