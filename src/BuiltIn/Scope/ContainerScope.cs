using System.Diagnostics;
using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    [DebuggerDisplay("Identity = { Identity }, Manager = {Manager}", Name = "{ (Contract.Type?.Name ?? string.Empty),nq }")]
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

        protected int _registryMax;
        protected int _registryCount;
        protected Metadata[] _registryMeta;
        protected Registration[] _registryData;

        protected object _registrySync = new object();


        protected int _namesMax;
        protected int _namesPrime;
        protected int _namesCount;
        protected Metadata[] _namesMeta;
        protected NameInfo[] _namesData;

        protected object _namesSync = new object();

        #endregion


        #region Constructors

        internal ContainerScope()
            : base()
        {
            // Initial size
            _namesPrime = IDENTITY_ROOT_INDEX;

            // Registrations
            var size = Prime.Numbers[REGISTRY_ROOT_INDEX];
            _registryMeta = new Metadata[size];
            _registryData = new Registration[size];
            _registryMax  = (int)(size * LoadFactor);

            size = Prime.Numbers[_namesPrime];
            _namesMeta = new Metadata[size];
            _namesData = new NameInfo[size];
            _namesMax  = (int)(size * LoadFactor);
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope)
        {
            // Initial size
            _namesPrime = IDENTITY_CHILD_INDEX;

            // Registrations
            var size = Prime.Numbers[REGISTRY_CHILD_INDEX];
            _registryMeta = new Metadata[size];
            _registryData = new Registration[size];
            _registryMax = (int)(size * LoadFactor);

            size = Prime.Numbers[_namesPrime];
            _namesMeta = new Metadata[size];
            _namesData = new NameInfo[size];
            _namesMax = (int)(size * LoadFactor);
        }

        ~ContainerScope() => Dispose(false);

        #endregion
    }
}
