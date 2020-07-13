using System;
using Unity.Container;
using Unity.Lifetime;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope : Scope
    {
        #region Constants

        protected const float LoadFactor = 0.72f;

        protected const int START_DATA  = 4;
        protected const int START_INDEX = 1;
        protected const int HASH_CODE_SEED = 52361;
        protected const int DEFAULT_REGISTRY_PRIME = 1;
        protected const int DEFAULT_IDENTITY_PRIME = 0;
        protected const string ASYNC_ERROR_MESSAGE = "This feature requires 'Unity.Professional' extension";

        #endregion


        #region Fields

        protected readonly LifetimeManager _manager;

        protected int _contractMax;
        protected int _registryMax;
        protected int _contracts;
        protected int _contractPrime;
        protected int _registrations;
        protected Metadata[] _registryMeta;
        protected Registry[] _registryData;
        protected Metadata[] _contractMeta;
        protected Identity[] _contractData;

        #endregion


        #region Constructors

        internal ContainerScope(UnityContainer container, int registry = DEFAULT_REGISTRY_PRIME, 
                                                          int identity = DEFAULT_IDENTITY_PRIME)
            : base(container)
        {
            _manager = new ContainerLifetimeManager(container);

            // Initial size
            _contractPrime = identity;

            // Registrations
            var size = Prime.Numbers[registry];
            _registryMeta = new Metadata[size];
            _registryData = new Registry[size];
            _registryMax  = (int)(size * LoadFactor);

            size = Prime.Numbers[_contractPrime];
            _contractMeta = new Metadata[size];
            _contractData = new Identity[size];
            _contractMax  = (int)(size * LoadFactor);

            // Built-in types
            var type_0 = typeof(UnityContainer);
            var type_1 = typeof(IUnityContainer);
            var type_2 = typeof(IUnityContainerAsync);
            var type_3 = typeof(IServiceProvider);

            // Add built-in registrations
            _registryData[  _registrations] = new Registry((uint)type_0.GetHashCode(), type_0, _manager);
            _registryData[++_registrations] = new Registry((uint)type_1.GetHashCode(), type_1, _manager);
            _registryData[++_registrations] = new Registry((uint)type_2.GetHashCode(), type_2, _manager);
            _registryData[++_registrations] = new Registry((uint)type_3.GetHashCode(), type_3, _manager);

            // Rebuild Metadata
            for (var current = START_INDEX; current <= _registrations; current++)
            {
                var bucket = _registryData[current].Hash % _registryMeta.Length;
                _registryMeta[current].Next    = _registryMeta[bucket].Position;
                _registryMeta[bucket].Position = current;
            }
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope)
        {
            // Copy data
            _manager       = scope._manager;
            _contracts     = scope._contracts;
            _contractMax   = scope._contractMax;
            _registryMax   = scope._registryMax;
            _registryData  = scope._registryData;
            _contractData  = scope._contractData;
            _registryMeta  = scope._registryMeta;
            _contractMeta  = scope._contractMeta;
            _contractPrime = scope._contractPrime;
            _registrations = scope._registrations;
        }

        ~ContainerScope() => Dispose(false);

        #endregion
    }
}
