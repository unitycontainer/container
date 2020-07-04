using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        [DebuggerDisplay("Scope: Size = { _registryCount }", Name = "Scope { _container.Name }")]
        public partial class ContainerScope
        {
            #region Constants

            private const int HashMask = unchecked((int)(uint.MaxValue >> 1));
            protected const int START_INDEX = 1;
            protected const int START_DATA  = 4;
            protected const string ASYNC_ERROR_MESSAGE = "This feature requires 'Unity.Professional' extension";

            #endregion


            #region Fields

            protected readonly ICollection<IDisposable> _sync;

            protected int _registryPrime;
            protected int _identityPrime;
            protected int _identityCount;
            protected int _registryCount;
            protected Metadata[] _registryMeta;
            protected Metadata[] _identityMeta;
            protected readonly Registry[] _registryData;
            protected readonly Identity[] _identityData;

            #endregion


            #region Constructors

            internal ContainerScope(UnityContainer container)
            {
                // Scope specific
                _sync = new List<IDisposable>();
                Container = container;
                Parent = container._parent?._scope;

                // Registrations
                _identityPrime = null == Parent ? 3 : 1;
                _registryPrime = _identityPrime;
                
                var size = Prime.Numbers[_identityPrime];

                _identityMeta  = new Metadata[size];
                _registryMeta  = new Metadata[size];
                _identityData  = new Identity[size];
                _registryData  = new Registry[size];

                // Register Interfaces
                ref var zero   = ref _registryData[_registryCount++];
                ref var one    = ref _registryData[_registryCount++];
                ref var two    = ref _registryData[_registryCount++];
                ref var three  = ref _registryData[_registryCount];

                zero.Type    = typeof(UnityContainer);
                zero.Manager = new ContainerLifetimeManager(Container);
                one.Manager  = zero.Manager;
                one.Type     = typeof(IUnityContainer);
                one.HashCode = one.GetHashCode();
                two.Manager   = one.Manager;
                two.Type      = typeof(IUnityContainerAsync);
                two.HashCode  = two.GetHashCode();
                three.Manager   = two.Manager;
                three.Type      = typeof(IServiceProvider);
                three.HashCode  = three.GetHashCode();

                // Rebuild Metadata
                for (var index = START_INDEX; index <= _registryCount; index++)
                {
                    var bucket = _registryData[index].HashCode % size;
                    _registryMeta[index].Next = _registryMeta[bucket].Bucket;
                    _registryMeta[bucket].Bucket = index;
                }
            }

            // Copy constructor
            protected ContainerScope(ContainerScope scope)
            {
                // Ownership
                _sync      = scope._sync;
                Parent    = scope.Parent;
                Container = scope.Container;

                // Copy data
                _registryData  = scope._registryData;
                _identityData  = scope._identityData;
                _registryMeta  = scope._registryMeta;
                _identityMeta  = scope._identityMeta;
                _registryPrime = scope._registryPrime;
                _identityPrime = scope._identityPrime;
                _registryCount = scope._registryCount;
                _identityCount = scope._identityCount;
            }

            #endregion


            #region Public Members

            public readonly ContainerScope? Parent;
            public readonly UnityContainer Container;

            #endregion


            #region Child Scope

            /// <summary>
            /// Creates new scope for child container
            /// </summary>
            /// <param name="container"><see cref="UnityContainer"/> that owns the scope</param>
            /// <returns>New scope associated with the container</returns>
            public virtual ContainerScope CreateChildScope(UnityContainer container)
                => new ContainerScope(container);

            #endregion
        }
    }
}
