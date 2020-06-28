using Unity.Lifetime;

namespace Unity.Scope
{
    public partial class RegistrationScope
    {
        #region Fields

        protected RegistrationScope? _parent;

        #endregion


        #region Constructors

        internal RegistrationScope()
        {
            // Disposable Container
            LifetimeContainer = new LifetimeContainer();

            _typeLifetime     = new TransientLifetimeManager();
            _factoryLifetime  = _typeLifetime;
            _instanceLifetime = new ContainerControlledLifetimeManager();
        }

        protected RegistrationScope(RegistrationScope parent)
        {
            // Parent
            _parent = parent;

            // Disposable Container
            LifetimeContainer = new LifetimeContainer();

            // Default Lifetime
            _typeLifetime = _parent._typeLifetime;
            _factoryLifetime  = _parent._factoryLifetime;
            _instanceLifetime = _parent._instanceLifetime;
        }

        #endregion


        public virtual RegistrationScope CreateChildScope()
            => new RegistrationScope(this);
    }
}
