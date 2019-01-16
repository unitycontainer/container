using System;

namespace Unity
{
    /// <summary>
    /// A <see cref="LifetimeManager"/> that holds a weak reference to
    /// it's managed instance.
    /// </summary>
    public class ExternallyControlledLifetimeManager : LifetimeManager
    {
        private WeakReference _value = new WeakReference(null);

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">Instance of container requesting the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public override object? GetValue(ILifetimeContainer? container = null)
        {
            return _value.Target;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="container">Instance of container which owns the value</param>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue, ILifetimeContainer? container = null)
        {
            _value = new WeakReference(newValue);
        }

        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new ExternallyControlledLifetimeManager();
        }


        #region Overrides

        public override string ToString() => "Lifetime.External";

        #endregion
    }
}
