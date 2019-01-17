using System;
using System.Collections.Generic;

namespace Unity.Lifetime
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds the instances given to it, 
    /// keeping one instance per thread.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This LifetimeManager does not dispose the instances it holds.
    /// </para>
    /// </remarks>
    public class PerThreadLifetimeManager : LifetimeManager
    {
        [ThreadStatic]
        private static Dictionary<Guid, object> _values;
        private readonly Guid _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerThreadLifetimeManager"/> class.
        /// </summary>
        public PerThreadLifetimeManager()
        {
            _key = Guid.NewGuid();
        }

        /// <summary>
        /// Retrieve a value from the backing store associated with this WithLifetime policy for the 
        /// current thread.
        /// </summary>
        /// <param name="container">Instance of container requesting the value</param>
        /// <returns>the object desired, or <see langword="null"/> if no such object is currently 
        /// stored for the current thread.</returns>
        public override object GetValue(ILifetimeContainer container = null)
        {
            EnsureValues();

            _values.TryGetValue(_key, out var result);

            return result;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later when requested
        /// in the current thread.
        /// </summary>
        /// <param name="container">Instance of container which owns the value</param>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            EnsureValues();

            _values[_key] = newValue;
        }

        private static void EnsureValues()
        {
            // no need for locking, values is TLS
            if (_values == null)
            {
                _values = new Dictionary<Guid, object>();
            }
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new PerThreadLifetimeManager();
        }


        #region Overrides

        public override string ToString() => "WithLifetime.PerThread";

        #endregion
    }
}
