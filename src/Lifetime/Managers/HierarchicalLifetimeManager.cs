using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Lifetime
{
    /// <summary>
    /// A special lifetime manager which works like <see cref="ContainerControlledLifetimeManager"/>,
    /// except that in the presence of child containers, each child gets it's own instance
    /// of the object, instead of sharing one in the common parent.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Unity container allows creating hierarchies of child containers. This lifetime 
    /// creates local singleton for each level of the hierarchy. So, when you resolve a 
    /// type and this container does not have an instance of that type, the container will 
    /// create new instance. Next type the type is resolved the same instance will be returned.
    /// </para>
    /// <para>
    /// If a child container is created and requested to resolve the type, the child container 
    /// will create a new instance and store it for subsequent resolutions. Next time the 
    /// child container requested to resolve the type, it will return stored instance.
    /// </para>
    /// <para>If you have multiple children, each will resolve its own instance.</para>
    /// </remarks>
    public class HierarchicalLifetimeManager : SynchronizedLifetimeManager, 
                                               IFactoryLifetimeManager,
                                               ITypeLifetimeManager
    {
        #region Fields

        private readonly IDictionary<ILifetimeContainer, object> _values = 
            new ConcurrentDictionary<ILifetimeContainer, object>();

        #endregion


        #region Overrides

        /// <inheritdoc/>
        protected override object SynchronizedGetValue(ILifetimeContainer container = null)
        {
            return _values.TryGetValue(container ?? throw new ArgumentNullException(nameof(container)), 
                                       out object value) ? value : NoValue;
        }

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object newValue, ILifetimeContainer container = null)
        {
            _values[container ?? throw new ArgumentNullException(nameof(container))] = newValue;
            container.Add(new DisposableAction(() => RemoveValue(container)));
        }


        /// <inheritdoc/>
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

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager()
        {
            return new HierarchicalLifetimeManager();
        }

        /// <summary>
        /// This method provides human readable representation of the lifetime
        /// </summary>
        /// <returns>Name of the lifetime</returns>
        public override string ToString() => "Lifetime:Hierarchical";

        #endregion


        #region IDisposable

        /// <inheritdoc/>
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
