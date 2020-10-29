using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Injection;

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

        private readonly IDictionary<ICollection<IDisposable>, object?> _values = 
            new ConcurrentDictionary<ICollection<IDisposable>, object?>();

        #endregion


        #region Constructors

        public HierarchicalLifetimeManager(params InjectionMember[] members)
            : base(members)
        {
        }

        #endregion


        #region Overrides

        /// <inheritdoc/>
        protected override object? SynchronizedGetValue(ICollection<IDisposable> scope) 
            => _values.TryGetValue(scope, out object? value) ? value : NoValue;

        /// <inheritdoc/>
        protected override void SynchronizedSetValue(object? newValue, ICollection<IDisposable> scope)
        {
            _values[scope] = newValue;
            scope.Add(new DisposableAction(() => RemoveValue(scope)));
        }

        /// <inheritdoc/>
        public override ResolutionStyle Style 
            => ResolutionStyle.OnceInWhile;

        /// <inheritdoc/>
        protected override LifetimeManager OnCreateLifetimeManager() 
            => new HierarchicalLifetimeManager();

        /// <inheritdoc/>
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

        private void RemoveValue(ICollection<IDisposable> scope)
        {
            if (!_values.TryGetValue(scope, out object? value)) return;

            _values.Remove(scope);
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        #endregion


        #region Nested Types

        private class DisposableAction : IDisposable
        {
            private readonly Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
            }
        }

        #endregion
    }
}
