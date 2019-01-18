namespace Unity.Lifetime
{
    public interface ILifetimeManager
    {
        /// <summary>
        /// Retrieve a value from the backing store associated with this WithLifetime policy.
        /// </summary>
        /// <param name="container">Child container this lifetime belongs to</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        object GetValue(ILifetimeContainer container = null);

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">The container this lifetime belongs to</param>
        void SetValue(object newValue, ILifetimeContainer container = null);

        /// <summary>
        /// Remove the given object from backing store.
        /// <param name="container">The container this lifetime belongs to</param>
        /// </summary>
        void RemoveValue(ILifetimeContainer container = null);
    }
}
