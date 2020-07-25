using System;
using System.Collections.Generic;
using Unity.Storage;

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

        protected const int PRIME_ROOT_INDEX  = 2;
        protected const int PRIME_CHILD_INDEX = 0;

        #endregion


        #region Fields

        // Entire scope lock
        protected readonly object _syncRoot;

        // Names
        protected int _namesPrime;
        protected int _namesCount;
        protected Metadata[] _namesMeta;
        protected NameInfo[] _namesData;

        // Contracts
        protected int _contractCount;
        protected ContainerRegistration[] _contractData;

        // Scope info
        protected int _version;
        protected readonly ICollection<IDisposable> _disposables;

        #endregion


        #region Constructors

        /// <summary>
        /// Root scope
        /// </summary>
        protected internal Scope()
        {
            // Scope lock
            _syncRoot = new object();

            // Names
            _namesPrime = PRIME_ROOT_INDEX;
            _namesMeta = new Metadata[Prime.Numbers[_namesPrime]];
            _namesMeta.Setup(LoadFactor);
            _namesData = new NameInfo[_namesMeta.Capacity()];

            // Contracts
            _contractData = new ContainerRegistration[Prime.Numbers[PRIME_ROOT_INDEX]];

            // Segment
            _disposables = new List<IDisposable>();
            Parent = null;

        }

        /// <summary>
        /// Child scope
        /// </summary>
        /// <param name="parent">parent scope</param>
        protected Scope(Scope parent)
        {
            // Scope lock
            _syncRoot = new object();

            // Names
            _namesMeta = new Metadata[Prime.Numbers[_namesPrime]];
            _namesMeta.Setup(LoadFactor);
            _namesData = new NameInfo[_namesMeta.Capacity()];

            // Contracts
            _contractData = new ContainerRegistration[Prime.Numbers[PRIME_CHILD_INDEX]];

            // Segment
            _disposables = new List<IDisposable>();
            Parent = parent;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="scope">Scope to copy from</param>
        /// <param name="sync">Synchronization root</param>
        protected internal Scope(Scope scope, object sync)
        {
            // Scope lock
            _syncRoot = sync;

            // Names
            _namesPrime = scope._namesPrime;
            _namesCount = scope._namesCount;
            _namesMeta  = scope._namesMeta;
            _namesData  = scope._namesData;

            // Contracts
            _contractCount = scope._contractCount;
            _contractData = scope._contractData;

            // Info
            _version = scope._version;

            // Segment
            _disposables = scope._disposables;
            Parent = scope.Parent;
        }

        #endregion
    }
}
