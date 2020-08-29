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

        #endregion


        #region Fields

        // Entire scope lock
        protected readonly object Sync;

        // Names
        protected int NamesPrime;
        protected int NamesCount;
        [CLSCompliant(false)] protected Metadata[] NamesMeta;
        [CLSCompliant(false)] protected NameInfo[] NamesData;

        // Contracts
        protected int ContractsPrime;
        protected int ContractsCount;
        protected ContainerRegistration[] ContractsData;

        // Scope info
        [CLSCompliant(false)] protected int _version;

        #endregion


        #region Constructors

        /// <summary>
        /// Root scope
        /// </summary>
        protected internal Scope(int capacity)
        {
            // Scope lock
            Sync = new object();

            // Names
            NamesPrime = 2;
            NamesData = new NameInfo[Prime.Numbers[NamesPrime++]];
            NamesMeta = new Metadata[Prime.Numbers[NamesPrime]];

            // Contracts
            ContractsPrime = Prime.IndexOf(capacity);
            ContractsData = new ContainerRegistration[Prime.Numbers[ContractsPrime++]];

            // Segment
            Disposables = new List<IDisposable>();
            Next = null;

        }

        /// <summary>
        /// Child scope
        /// </summary>
        /// <param name="parent">parent scope</param>
        protected Scope(Scope parent, int capacity)
        {
            // Scope lock
            Sync = new object();

            // Names
            NamesData = new NameInfo[Prime.Numbers[NamesPrime++]];
            NamesMeta = new Metadata[Prime.Numbers[NamesPrime]];

            // Contracts
            ContractsPrime = Prime.IndexOf(capacity);
            ContractsData = new ContainerRegistration[Prime.Numbers[ContractsPrime++]];

            // Segment
            Disposables = new List<IDisposable>();
            Next = parent;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="scope">Scope to copy from</param>
        /// <param name="sync">Synchronization root</param>
        protected internal Scope(Scope scope, object sync)
        {
            // Scope lock
            Sync = sync;

            // Names
            NamesPrime = scope.NamesPrime;
            NamesCount = scope.NamesCount;
            NamesMeta  = scope.NamesMeta;
            NamesData  = scope.NamesData;

            // Contracts
            ContractsPrime = scope.ContractsPrime;
            ContractsCount = scope.ContractsCount;
            ContractsData = scope.ContractsData;

            // Info
            _version = scope._version;

            // Segment
            Disposables = scope.Disposables;
            Next = scope.Next;
        }

        #endregion
    }
}
