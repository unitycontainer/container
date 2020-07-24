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

        // Entire scope lock
        protected object _scopeLock;

        // Names
        protected int _namesPrime;
        protected int _namesCount;
        protected Metadata[] _namesMeta;
        protected NameInfo[] _namesData;

        // Registrations
        protected int _contractCount;
        protected Metadata[] _contractMeta;
        protected ContainerRegistration[] _contractData;

        #endregion


        #region Constructors

        // Root constructor
        internal ContainerScope()
            : base()
        {
            // Entire scope lock
            _scopeLock = new object();

            // Names
            _namesPrime = PRIME_ROOT_INDEX;
            _namesMeta = new Metadata[Prime.Numbers[_namesPrime]];
            _namesMeta.Setup(LoadFactor);
            _namesData = new NameInfo[_namesMeta.Capacity()];

            // Registrations
            _contractMeta = new Metadata[Prime.Numbers[PRIME_ROOT_INDEX]];
            _contractMeta.Setup(LoadFactor);
            _contractData = new ContainerRegistration[_contractMeta.Capacity()];
        }

        // Child constructor
        protected ContainerScope(Scope scope)
            : base(scope)
        {
            _scopeLock = new object();

            // Names
            _namesMeta = new Metadata[Prime.Numbers[_namesPrime]];
            _namesMeta.Setup(LoadFactor);
            _namesData = new NameInfo[_namesMeta.Capacity()];

            // Registrations
            _contractMeta = new Metadata[Prime.Numbers[PRIME_CHILD_INDEX]];
            _contractMeta.Setup(LoadFactor);
            _contractData = new ContainerRegistration[_contractMeta.Capacity()];
        }

        // Copy constructor
        protected ContainerScope(ContainerScope scope)
            : base(scope)
        {
            _scopeLock = scope._scopeLock;

            // Names
            _namesPrime = scope._namesPrime;
            _namesCount = scope._namesCount;
            _namesMeta  = scope._namesMeta;
            _namesData  = scope._namesData;

            // Registrations
            _contractCount = scope._contractCount;
            _contractMeta  = scope._contractMeta;
            _contractData  = scope._contractData;
        }

        ~ContainerScope() => Dispose(false);

        #endregion
    }
}
