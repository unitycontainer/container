using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Storage;

namespace Unity.Container
{
    [DebuggerDisplay("Size = { _registryCount }", Name = "Scope({ Container.Name })")]
    public partial class ContainerScope
    {
        #region Constants

        protected const float LoadFactor = 0.72f;

        protected const int START_DATA  = 4;
        protected const int START_COUNT = 3;
        protected const int START_INDEX = 1;
        protected const int IDENTITY_SIZE = 3; 
        protected const int DEFAULT_REGISTRY_SIZE = 1;
        protected const int DEFAULT_IDENTITY_SIZE = 0;

        protected const string ASYNC_ERROR_MESSAGE = "This feature requires 'Unity.Professional' extension";

        #endregion


        #region Fields

        protected readonly ICollection<IDisposable> _lifetimes;

        protected int _identityMax;
        protected int _registryMax;
        protected int _identityPrime;
        protected int _identityCount;
        protected int _registryCount;
        protected Metadata[] _identityMeta;
        protected Metadata[] _registryMeta;
        protected Identity[] _identityData;
        protected Registry[] _registryData;
        protected readonly object _registrySync;
        protected readonly object _identitySync;

        #endregion


        #region Constructors

        internal ContainerScope(UnityContainer container, int registry = DEFAULT_REGISTRY_SIZE, 
                                                          int identity = DEFAULT_IDENTITY_SIZE)
        {
            Parent    = container.Parent?._scope;
            Container = container;

            // Scope specific
            _registrySync = new object();
            _identitySync = new object();
            _lifetimes = new List<IDisposable>();

            // Initial size
            _identityPrime = identity;
            
            // Registrations
            var size = Prime.Numbers[registry];
            _registryMeta = new Metadata[size];
            _registryData = new Registry[size];
            _registryMax  = (int)(size * LoadFactor);

            size = Prime.Numbers[_identityPrime];
            _identityMeta = new Metadata[size];
            _identityData = new Identity[size];
            _identityMax  = (int)(size * LoadFactor);

            // Add Interface registrations
            ref var zero  = ref _registryData[_registryCount++];
            ref var one   = ref _registryData[_registryCount++];
            ref var two   = ref _registryData[_registryCount++];
            ref var three = ref _registryData[_registryCount];

            // Setup Local registrations
            zero.Type = typeof(UnityContainer);
            zero.Manager = new ContainerLifetimeManager(Container);

            one.Manager = zero.Manager;
            one.Type = typeof(IUnityContainer);
            one.Hash = (uint)one.GetHashCode();

            two.Manager = one.Manager;
            two.Type = typeof(IUnityContainerAsync);
            two.Hash = (uint)two.GetHashCode();

            three.Manager = two.Manager;
            three.Type = typeof(IServiceProvider);
            three.Hash = (uint)three.GetHashCode();

            // Rebuild Metadata
            for (var index = START_INDEX; index <= _registryCount; index++)
            {
                var bucket = _registryData[index].Hash % size;
                _registryMeta[index].Next = _registryMeta[bucket].Bucket;
                _registryMeta[bucket].Bucket = index;
            }
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
        {
            // Copy data
            Parent         = scope.Parent;
            Container      = scope.Container;

            _lifetimes     = scope._lifetimes;
            _registrySync  = scope._registrySync;
            _identitySync  = scope._identitySync;
            _identityMax   = scope._identityMax;
            _registryMax   = scope._registryMax;
            _registryData  = scope._registryData;
            _identityData  = scope._identityData;
            _registryMeta  = scope._registryMeta;
            _identityMeta  = scope._identityMeta;
            _identityPrime = scope._identityPrime;
            _registryCount = scope._registryCount;
            _identityCount = scope._identityCount;
        }

        #endregion
    }
}
