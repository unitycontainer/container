

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Lifetime;

namespace Unity.Container.Lifetime
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
        private static readonly IDisposable[] EmptyDisposables = new IDisposable[0];

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
                if (_items.Count == 0)
                    disposables = EmptyDisposables;
                else
                {
                    disposables = new IDisposable[_items.Count];
                    for (var i = 0; i < _items.Count; i++)
                    {
                        //filter all non disposables
                        if (!(_items[i] is IDisposable disposable)) continue;

                        var alreadyAdded = false;
                        for (var j = 0; j < i; j++)
                        {
                            if (!Equals(disposables[j], disposable)) continue;

                            alreadyAdded = true;
                            break;
                        }

                        if (!alreadyAdded)
                            disposables[_items.Count - i - 1] = disposable;
                    }

                    _items.Clear();
                }
            }

            if (disposables.Length == 0)
                return;

            List<Exception> exceptions = null;
            foreach (var disposable in disposables)
            {
                if (disposable == null)
                    continue;

                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    if (exceptions == null)
                        exceptions = new List<Exception>();

                    exceptions.Add(e);
                }
            }

            if (exceptions == null)
                return;

            if (exceptions.Count == 1)
            {
                throw exceptions[0];
            }
            if (exceptions.Count > 1)
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
            lock (_items)
            {
                return _items.GetEnumerator();
            }
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
