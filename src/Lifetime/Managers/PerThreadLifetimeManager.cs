using System;
using System.Collections.Generic;

namespace Unity.Lifetime
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that creates a new instance of 
    /// the registered <see cref="Type"/> once per each thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Per thread lifetime means a new instance of the registered <see cref="Type"/>
    /// will be created once per each thread. In other words, if a Resolve{T}() method 
    /// is called on a thread the first time, it will return a new object. Each
    /// subsequent call to Resolve{T}(), or when the dependency mechanism injects 
    /// instances of the type into other classes on the same thread, the container 
    /// will return the same object.
    /// </para>
    /// <para>
    /// This LifetimeManager does not dispose the instances it holds.
    /// </para>
    /// </remarks>
    public class PerThreadLifetimeManager : LifetimeManager,
                                            IFactoryLifetimeManager,
                                            ITypeLifetimeManager
    {
        #region Fields

        [ThreadStatic]
        private static Dictionary<Guid, object> _values;
        private readonly Guid _key = Guid.NewGuid();

        #endregion


        #region Overrides

        /// <inheritdoc/>
        public override object GetValue(ILifetimeContainer container = null)
        {
            if (null == _values) return NoValue;

            return _values.TryGetValue(_key, out var result) ? result : NoValue;
        }

        /// <inheritdoc/>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            // no need for locking, values is TLS
            if (_values == null)
                _values = new Dictionary<Guid, object>();

            _values[_key] = newValue;
        }

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerThreadLifetimeManager();
        }



        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:PerThread";

        #endregion
    }
}
