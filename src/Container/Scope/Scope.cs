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

        protected int _contractMax;
        protected int _registryMax;
        protected int _contractPrime;
        protected int _contractCount;
        protected int _registryCount;
        protected Metadata[] _contractMeta;
        protected Metadata[] _registryMeta;
        protected Contract[] _contractData;
        protected Registry[] _registryData;
        protected readonly object _registrySync;
        protected readonly object _contractSync;

        #endregion


        #region Constructors

        internal ContainerScope(UnityContainer container, int registry = DEFAULT_REGISTRY_SIZE, 
                                                          int identity = DEFAULT_IDENTITY_SIZE)
        {
            Parent    = container.Parent?._scope;
            Container = container;

            // Scope specific
            _registrySync = new object();
            _contractSync = new object();
            _lifetimes = new List<IDisposable>();

            // Initial size
            _contractPrime = identity;
            
            // Registrations
            var size = Prime.Numbers[registry];
            _registryMeta = new Metadata[size];
            _registryData = new Registry[size];
            _registryMax  = (int)(size * LoadFactor);

            size = Prime.Numbers[_contractPrime];
            _contractMeta = new Metadata[size];
            _contractData = new Contract[size];
            _contractMax  = (int)(size * LoadFactor);

            // Add Interface registrations
            var manager = new ContainerLifetimeManager(Container);
            _registryData[  _registryCount] = new Registry((uint)typeof(UnityContainer      ).GetHashCode(), typeof(UnityContainer      ), manager);
            _registryData[++_registryCount] = new Registry((uint)typeof(IUnityContainer     ).GetHashCode(), typeof(IUnityContainer     ), manager);
            _registryData[++_registryCount] = new Registry((uint)typeof(IUnityContainerAsync).GetHashCode(), typeof(IUnityContainerAsync), manager);
            _registryData[++_registryCount] = new Registry((uint)typeof(IServiceProvider    ).GetHashCode(), typeof(IServiceProvider    ), manager);

            // Rebuild Metadata
            for (var index = START_INDEX; index <= _registryCount; index++)
            {
                var bucket = _registryData[index].Hash % size;
                _registryMeta[index].Next = _registryMeta[bucket].Position;
                _registryMeta[bucket].Position = index;
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
            _contractSync  = scope._contractSync;
            _contractMax   = scope._contractMax;
            _registryMax   = scope._registryMax;
            _registryData  = scope._registryData;
            _contractData  = scope._contractData;
            _registryMeta  = scope._registryMeta;
            _contractMeta  = scope._contractMeta;
            _contractPrime = scope._contractPrime;
            _registryCount = scope._registryCount;
            _contractCount = scope._contractCount;
        }

        #endregion
    }
}
