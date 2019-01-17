using System;
using System.Collections.Generic;

namespace Unity.Lifetime
{
    /// <summary>
    /// Represents a lifetime container.
    /// </summary>
    /// <remarks>
    /// A lifetime container tracks the lifetime of an object, and implements
    /// IDisposable. When the container is disposed, any objects in the
    /// container which implement IDisposable are also disposed.
    /// </remarks>
    public interface ILifetimeContainer : IEnumerable<object>, IDisposable
    {
        /// <summary>
        /// The container that this context is associated with.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> object.</value>
        IUnityContainer Container { get; }

        /// <summary>
        /// Gets the number of references in the lifetime container
        /// </summary>
        /// <value>
        /// The number of references in the lifetime container
        /// </value>
        int Count { get; }

        /// <summary>
        /// Adds an object to the lifetime container.
        /// </summary>
        /// <param name="item">The item to be added to the lifetime container.</param>
        void Add(object item);

        /// <summary>
        /// Determine if a given object is in the lifetime container.
        /// </summary>
        /// <param name="item">
        /// The item to locate in the lifetime container.
        /// </param>
        /// <returns>
        /// Returns true if the object is contained in the lifetime
        /// container; returns false otherwise.
        /// </returns>
        bool Contains(object item);

        /// <summary>
        /// Removes an item from the lifetime container. The item is
        /// not disposed.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        void Remove(object item);
    }
}
