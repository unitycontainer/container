namespace Unity.Container
{
    public abstract partial class Scope
    {
        #region Constants

        public const float LoadFactor = 0.75f;
        public const float ReLoadFactor = 1.45f;

        protected const int START_DATA  = 4;
        protected const int START_INDEX = 1;
        protected const int HASH_CODE_SEED = 52361;

        #endregion


        #region Fields

        public readonly int Level;
        protected readonly object SyncRoot;
        private readonly HashSet<IDisposable> _disposables;

        // Contracts
        protected int Prime;
        protected int Index;
        protected int Revision;
        protected Entry[] Data;
        internal readonly Scope[] Ancestry;

        #endregion


        #region Constructors

        /// <summary>
        /// Root scope
        /// </summary>
        protected internal Scope(int capacity)
        {
            // Scope lock
            SyncRoot = new object();

            // Contracts
            Prime = Storage.Prime.IndexOf(capacity);
#if NETSTANDARD
            Data = new Entry[Storage.Prime.Numbers[Prime++]];
#else
            Data  = GC.AllocateUninitializedArray<Entry>(Storage.Prime.Numbers[Prime++], false);
#endif

            // Segment
            _disposables = new HashSet<IDisposable>();
            Next  = null;
            Level = 0;
            Ancestry = new[] { this };
        }

        /// <summary>
        /// Child scope
        /// </summary>
        /// <param name="parent">parent scope</param>
        protected Scope(Scope parent, int capacity)
        {
            // Scope lock
            SyncRoot = new object();

            // Contracts
            Prime = Storage.Prime.IndexOf(capacity);
#if NETSTANDARD
            Data = new Entry[Storage.Prime.Numbers[Prime++]];
#else
            Data  = GC.AllocateUninitializedArray<Entry>(Storage.Prime.Numbers[Prime++], false);
#endif

            // Segment
            _disposables = new HashSet<IDisposable>();
            Next  = parent;
            Level = parent.Level + 1;
            Ancestry = new Scope[parent.Ancestry.Length + 1];
            Ancestry[parent.Ancestry.Length] = this;
            parent.Ancestry.CopyTo(Ancestry, 0);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="scope">Scope to copy from</param>
        /// <param name="sync">Synchronization root</param>
        protected internal Scope(Scope scope, object sync)
        {
            // Scope lock
            SyncRoot = sync;

            // Contracts
            Prime = scope.Prime;
            Index = scope.Index;
            Data = scope.Data;

            // Info
            Revision = scope.Revision;

            // Segment
            _disposables = scope._disposables;
            Next  = scope.Next;
            Level = scope.Level;
            Ancestry = scope.Ancestry;
            Ancestry[scope.Ancestry.Length - 1] = this;
        }

        #endregion
    }
}
