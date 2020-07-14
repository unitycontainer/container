using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope : Scope
    {
        #region Constants

        protected const int START_DATA  = 4;
        protected const int START_INDEX = 1;
        protected const int HASH_CODE_SEED = 52361;

        protected const int REGISTRY_ROOT_INDEX = 3;
        protected const int IDENTITY_ROOT_INDEX = 3;
        protected const int REGISTRY_CHILD_INDEX = 1;
        protected const int IDENTITY_CHILD_INDEX = 0;

        protected const string ASYNC_ERROR_MESSAGE = "This feature requires 'Unity.Professional' extension";

        #endregion


        #region Fields

        protected int _identityMax;
        protected int _registryMax;
        protected int _identityPrime;
        protected int _identityCount;
        protected int _registryCount;
        protected Metadata[] _registryMeta;
        protected Registry[] _registryData;
        protected Metadata[] _identityMeta;
        protected Identity[] _identityData;

        protected object _registrySync = new object();
        protected object _contractSync = new object();

        #endregion


        #region Constructors

        internal ContainerScope()
            : base()
        {
            // Initial size
            _identityPrime = IDENTITY_ROOT_INDEX;

            // Registrations
            var size = Prime.Numbers[REGISTRY_ROOT_INDEX];
            _registryMeta = new Metadata[size];
            _registryData = new Registry[size];
            _registryMax  = (int)(size * LoadFactor);

            size = Prime.Numbers[_identityPrime];
            _identityMeta = new Metadata[size];
            _identityData = new Identity[size];
            _identityMax  = (int)(size * LoadFactor);
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope)
        {
            // Initial size
            _identityPrime = IDENTITY_CHILD_INDEX;

            // Registrations
            var size = Prime.Numbers[REGISTRY_CHILD_INDEX];
            _registryMeta = new Metadata[size];
            _registryData = new Registry[size];
            _registryMax = (int)(size * LoadFactor);

            size = Prime.Numbers[_identityPrime];
            _identityMeta = new Metadata[size];
            _identityData = new Identity[size];
            _identityMax = (int)(size * LoadFactor);
        }

        ~ContainerScope() => Dispose(false);

        #endregion
    }
}
