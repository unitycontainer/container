// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    public class HierarchicalLifetimeManager : SynchronizedLifetimeManager, 
                                               IHierarchicalLifetimePolicy
    {
        #region Fields

        private readonly IDictionary<ILifetimeContainer, object> _values = new Dictionary<ILifetimeContainer, object>();

        #endregion

        /// <summary>
        /// Performs the actual retrieval of a value from the backing store associated 
        /// with this Lifetime policy.
        /// </summary>
        /// <param name="container">Container that owns the value</param>
        /// <returns>the object desired, or null if no such object is currently stored.</returns>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.GetValue"/>
        /// after it has acquired its lock.</remarks>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return _values.TryGetValue(container ?? throw new ArgumentNullException(nameof(container)), 
                                       out object value) ? value : null;
        }

        /// <summary>
        /// Performs the actual storage of the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        /// <param name="container">Container that owns the value</param>
        /// <remarks>This method is invoked by <see cref="SynchronizedLifetimeManager.SetValue"/>
        /// before releasing its lock.</remarks>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            _values[container ?? throw new ArgumentNullException(nameof(container))] = newValue;
            container.Add(new DisposableAction(() => RemoveValue(container)));
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
        /// Standard Dispose pattern implementation.
        /// </summary>
        /// <param name="disposing">Always true, since we don't have a finalizer.</param>
        protected override void Dispose(bool disposing)
        {
            try
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
            finally 
            {
                base.Dispose(disposing);
            }
        }

        #endregion


        #region Nested Types

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
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
