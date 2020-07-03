using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        [DebuggerDisplay("Registrations")]
        [DebuggerTypeProxy(typeof(EnumerableDebugProxy<ContainerScope, ContainerRegistration>))]
        public partial class ContainerScope
        {
            #region Constants

            private const int HashMask = unchecked((int)(uint.MaxValue >> 1));

            #endregion


            #region Fields

            protected ContainerScope? _parent;
            protected UnityContainer  _container;
            protected ICollection<IDisposable> _sync = new List<IDisposable>();

            #endregion


            #region Constructors

            internal ContainerScope(UnityContainer container)
            {
                _container = container;
                _parent = container._parent?._scope;

                // Root Container
                if (null == _parent)
                { 
                    _registryPrime = 2;
                    CreateMetadata(1);
                }

                // Create registry
                var length = Prime.Numbers[_registryPrime];
                _registry = new RegistryEntry[length];
                _registryBuckets = new int[length];
                _registryBuckets.Fill(-1);

                // Register IUnityContainer
                Add(typeof(IUnityContainer), new ContainerLifetimeManager(container));
            }

            #endregion


            #region Implementation

            public virtual ContainerScope CreateChildScope(UnityContainer container)
                => new ContainerScope(container);

            #endregion
        }
    }
}
