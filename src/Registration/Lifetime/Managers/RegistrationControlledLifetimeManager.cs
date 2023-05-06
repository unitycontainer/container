using Unity.Injection;
using Unity.Storage;

namespace Unity.Lifetime
{
    public class RegistrationControlledLifetimeManager : SynchronizedLifetimeManager, 
                                                         IFactoryLifetimeManager,
                                                         ITypeLifetimeManager
    {
        #region Fields

        /// <summary>
        /// An instance of the object this manager is associated with.
        /// </summary>
        /// <value>This field holds a strong reference to the associated object.</value>
        protected object? Value = UnityContainer.NoValue;
        
        private int _version = -1;

        #endregion


        #region Constructor

        public RegistrationControlledLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object? TryGetValue(ILifetimeContainer scope)
        {
            if (((Scope)scope).Version != _version) return UnityContainer.NoValue;

            return Value;
        }

        /// <inheritdoc/>
        protected override object? SynchronizedGetValue(ILifetimeContainer scope)
        {
            if (((Scope)scope).Version != _version) return UnityContainer.NoValue;

            return Value;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object? newValue, ILifetimeContainer scope)
        {
            Value = newValue;
            _version = ((Scope)scope).Version;
        }

        /// <inheritdoc/>
        public override CreationPolicy CreationPolicy 
            => CreationPolicy.Always;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new RegistrationControlledLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerRegistration"; 

        #endregion
    }
}
