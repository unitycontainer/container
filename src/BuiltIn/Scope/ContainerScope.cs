using Unity.Container;
using Unity.Storage;

namespace Unity.BuiltIn
{
    public partial class ContainerScope : Scope
    {
        #region Constants

        public const float LoadFactor   = 0.75f;
        public const float ReLoadFactor = 1.45f;

        protected const int START_DATA  = 4;
        protected const int START_INDEX = 1;
        protected const int HASH_CODE_SEED = 52361;

        protected const int PRIME_ROOT_INDEX  = 3;
        protected const int PRIME_CHILD_INDEX = 1;

        protected const string ASYNC_ERROR_MESSAGE = "This feature requires 'Unity.Professional' extension";

        #endregion


        #region Fields


        // Names
        protected int _namesPrime;
        protected int _namesCount;
        protected Metadata[] _namesMeta;
        protected NameInfo[] _namesData;
        //private object _syncRoot = new object();
        private System.Threading.ReaderWriterLockSlim _registryLock = new ReaderWriterLockSlim();

        // Registrations
        protected int _registryCount;
        protected Metadata[] _registryMeta;
        protected Registration[] _registryData;

        #endregion


        #region Constructors

        internal ContainerScope()
            : base()
        {
            // Names
            _namesPrime = PRIME_ROOT_INDEX;
            _namesMeta = new Metadata[Prime.Numbers[_namesPrime]];
            _namesMeta.Setup(LoadFactor);
            _namesData = new NameInfo[_namesMeta.GetCapacity()];

            // Registrations
            _registryMeta = new Metadata[Prime.Numbers[PRIME_ROOT_INDEX]];
            _registryMeta.Setup(LoadFactor);
            _registryData = new Registration[_registryMeta.GetCapacity()];
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope)
        {
            // Names
            _namesMeta = new Metadata[Prime.Numbers[_namesPrime]];
            _namesMeta.Setup(LoadFactor);
            _namesData = new NameInfo[_namesMeta.GetCapacity()];

            // Registrations
            _registryMeta = new Metadata[Prime.Numbers[PRIME_CHILD_INDEX]];
            _registryMeta.Setup(LoadFactor);
            _registryData = new Registration[_registryMeta.GetCapacity()];
        }

        ~ContainerScope() => Dispose(false);

        #endregion
    }
}
