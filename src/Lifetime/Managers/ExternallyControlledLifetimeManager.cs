using System;

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

        private WeakReference _value;

        #endregion


        #region SynchronizedLifetimeManager

        /// <inheritdoc/>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            if (null == _value) return NoValue;

            var target = _value.Target;
            if (null != target) return target;

            _value = null;

            return NoValue;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            _value = new WeakReference(newValue);
        }


        /// <inheritdoc/>
        public override void RemoveValue(ILifetimeContainer container = null) => _value = null;

        #endregion


        #region Overrides

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ExternallyControlledLifetimeManager();
        }

        public override string ToString() => "Lifetime:External";

        #endregion
    }
}
