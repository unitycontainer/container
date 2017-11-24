// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.Exceptions;
using Unity.Policy;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    public class HierarchicalLifetimeManager : LifetimeManager, 
                                               IRequiresRecovery,
                                               IHierarchicalLifetimePolicy,
                                               IDisposable
    {
        #region Fields

        private readonly IDictionary<ILifetimeContainer, object> _values = new Dictionary<ILifetimeContainer, object>();

        #endregion

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <param name="container">Container that owns the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        /// <remarks>Calls to this method acquire a lock which is released only if a non-null value
        /// has been set for the lifetime manager.</remarks>
        public override object GetValue(ILifetimeContainer container = null)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));
            Monitor.Enter(_values);
            var result = SynchronizedGetValue(container);
            if (result != null)
            {
                Monitor.Exit(_values);
            }
            return result;
        }

        /// <summary>
        /// Performs the actual retrieval of a value from the backing store associated 
        /// with this Lifetime policy.
        /// </summary>
        /// <param name="container">Container that owns the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.GetValue"/>
        /// after it has acquired its lock.</remarks>
        protected virtual object SynchronizedGetValue(ILifetimeContainer container)
        {
            return _values.TryGetValue(container, out object value) ? value : null;
        }


        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">Container that owns the value</param>
        /// <remarks>Setting a value will attempt to release the lock acquired by 
        /// <see cref="SynchronizedLifetimeManager.GetValue"/>.</remarks>
        public override void SetValue(object newValue, ILifetimeContainer container = null)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));
            SynchronizedSetValue(newValue, container);
            TryExit();
        }

        /// <summary>
        /// Performs the actual storage of the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">Container that owns the value</param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.SetValue"/>
        /// before releasing its lock.</remarks>
        protected virtual void SynchronizedSetValue(object newValue, ILifetimeContainer container)
        {
            _values[container] = newValue;
            container.Add(new LifetimeProxy(() => RemoveValue(container)));
        }

        /// <summary>
        /// A method that does whatever is needed to clean up
        /// as part of cleaning up after an exception.
        /// </summary>
        /// <remarks>
        /// Don't do anything that could throw in this method,
        /// it will cause later recover operations to get skipped
        /// and play real havoc with the stack trace.
        /// </remarks>
        public void Recover()
        {
            TryExit();
        }

        private void TryExit()
        {
#if !NET40
            // Prevent first chance exception when abandoning a lock that has not been entered
            if (!Monitor.IsEntered(_values)) return;
#endif
            try
            {
                Monitor.Exit(_values);
            }
            catch (SynchronizationLockException)
            {
                // Noop here - we don't hold the lock and that's ok.
            }
        }


        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue(ILifetimeContainer container = null)
        {
            if (null == container) throw new ArgumentNullException(nameof(container));
            if (!_values.TryGetValue(container, out object value)) return;

            _values.Remove(container);

            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }


        #region IDisposable

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Standard Dispose pattern implementation.
        /// </summary>
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (0 == _values.Count) return;

            foreach (var disposable in _values.Values
                                              .OfType<IDisposable>()
                                              .ToArray())
            {
                disposable.Dispose();
            }
            _values.Clear();
        }

        #endregion


        #region Nested Types

        private class LifetimeProxy : IDisposable
        {
            private readonly Action _action;

            public LifetimeProxy(Action action)
            {
                _action = action ?? throw new ArgumentNullException(nameof(action));
            }

            public void Dispose()
            {
                _action();
            }
        }

        #endregion
    }
}
