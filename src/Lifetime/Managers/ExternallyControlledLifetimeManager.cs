using System;
using System.Collections.Generic;
using Unity.Injection;

namespace Unity.Lifetime
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds a weak reference to
    /// it's managed instance.
    /// </summary>
    public class ExternallyControlledLifetimeManager : SynchronizedLifetimeManager,
                                                       IInstanceLifetimeManager,
                                                       ITypeLifetimeManager,
                                                       IFactoryLifetimeManager
    {
        #region Fields

        private WeakReference? _value;

        #endregion


        #region Constructors

        public ExternallyControlledLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        protected override object? SynchronizedGetValue(ICollection<IDisposable> lefetime)
        {
            if (null == _value) return NoValue;

            var target = _value.Target;
            if (_value.IsAlive) return target;

            _value = null;

            return NoValue;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object? newValue, ICollection<IDisposable> lefetime) 
            => _value = new WeakReference(newValue);

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.OnceInAWhile;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new ExternallyControlledLifetimeManager();

        /// <inheritdoc/>
        public override string ToString() 
            => "Lifetime:External";

        #endregion
    }
}
