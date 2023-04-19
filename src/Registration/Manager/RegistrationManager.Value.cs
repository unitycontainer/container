using Unity.Lifetime;

namespace Unity
{
    /// <summary>
    /// This structure holds data passed to container registration
    /// </summary>
    public abstract partial class RegistrationManager
    {
        #region Try/Get/Set Value

        /// <summary>
        /// Attempts to retrieve a value from the backing lifetime manager
        /// </summary>
        /// <remarks>
        /// This method does not block and does not acquire a lock on lifetime 
        /// synchronization objects primitives.
        /// </remarks>
        /// <param name="scope">The lifetime container this manager is associated with</param>
        /// <returns>The object stored with the manager or <see cref="NoValue"/></returns>
        public virtual object? TryGetValue(ILifetimeContainer scope) => UnityContainer.NoValue;

        /// <summary>
        /// Retrieves a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="scope">The container this lifetime is associated with</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        public virtual object? GetValue(ILifetimeContainer scope) => UnityContainer.NoValue;

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="scope">The container this lifetime is associated with</param>
        public virtual void SetValue(object? newValue, ILifetimeContainer scope) { }

        #endregion
    }
}
