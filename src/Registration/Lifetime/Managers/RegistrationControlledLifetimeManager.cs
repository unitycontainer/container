using System;
using System.Collections.Generic;
using Unity.Container;
using Unity.Injection;

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
        public override object? TryGetValue(ICollection<IDisposable> scope)
        {
            if (((Scope)scope).Version != _version) return UnityContainer.NoValue;

            return Value;
        }

        /// <inheritdoc/>
        protected override object? SynchronizedGetValue(ICollection<IDisposable> scope)
        {
            if (((Scope)scope).Version != _version) return UnityContainer.NoValue;

            return Value;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object? newValue, ICollection<IDisposable> scope)
        {
            Value = newValue;
            _version = ((Scope)scope).Version;
        }

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.OnceInWhile;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new RegistrationControlledLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:PerRegistration"; 

        #endregion
    }
}
