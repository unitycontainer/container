using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
    public class LifetimeContainer : ILifetimeContainer
    {
        private readonly List<object> _items = new List<object>();

        public LifetimeContainer(IUnityContainer owner = null)
        {
            Container = owner;
        }

        /// <summary>
        /// The IUnityContainer this container is associated with.
        /// </summary>
        /// <value>The <see cref="IUnityContainer"/> object.</value>
        public IUnityContainer Container { get; }


        /// <summary>
        /// Gets the number of references in the lifetime container
        /// </summary>
        /// <value>
        /// The number of references in the lifetime container
        /// </value>
        public int Count
        {
            get
            {
                lock (_items)
                {
                    return _items.Count;
                }
            }
        }

        /// <summary>
        /// Adds an object to the lifetime container.
        /// </summary>
        /// <param name="item">The item to be added to the lifetime container.</param>
        public void Add(object item)
        {
            lock (_items)
            {
                _items.Add(item);
            }
        }

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
        public bool Contains(object item)
        {
            lock (_items)
            {
                return _items.Contains(item);
            }
        }

        /// <summary>
        /// Releases the resources used by the <see cref="LifetimeContainer"/>. 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this); 
        }

        /// <summary>
        /// Releases the resources used by the <see cref="LifetimeContainer"/>. 
        /// </summary>
        /// <param name="disposing">
        /// true to release managed and unmanaged resources; false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            IDisposable[] disposables;

            lock (_items)
            {
                disposables = _items.OfType<IDisposable>()
                                    .Distinct()
                                    .Reverse()
                                    .ToArray();
                _items.Clear();
            }


            var exceptions = new List<Exception>();
            foreach (var disposable in disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }


            if (exceptions.Count == 1)
            {
                throw exceptions.First();
            }
            else if (exceptions.Count > 1)
            {
                throw new AggregateException(exceptions);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the lifetime container.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the life time container. 
        /// </returns>
        public IEnumerator<object> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the lifetime container.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerator"/> object that can be used to iterate through the life time container. 
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Removes an item from the lifetime container. The item is
        /// not disposed.
        /// </summary>
        /// <param name="item">The item to be removed.</param>
        public void Remove(object item)
        {
            lock (_items)
            {
                if (!_items.Contains(item))
                {
                    return;
                }

                _items.Remove(item);
            }
        }
    }
}
