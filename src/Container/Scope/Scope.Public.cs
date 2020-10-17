using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("{GetType().Name,nq}: Contracts = { Count }, Version = { Version }")]
    public abstract partial class Scope : ICollection
    {
        #region Properties

        /// <summary>
        /// Version of scope
        /// </summary>
        /// <remarks>
        /// Scope version is increased every time registrations changes
        /// </remarks>
        public int Version => null == Next 
            ? Revision : Next.Version + Revision;

        /// <summary>
        /// Return <see cref="ReadOnlyMemory{RegistrationInfo}"/> encapsulating
        /// all registered contracts.
        /// </summary>
        public ReadOnlyMemory<Entry> Memory 
            => new ReadOnlyMemory<Entry>(Data, 1, Index);

        /// <summary>
        /// Storage capacity of the scope
        /// </summary>
        public int Capacity => Data.Length;

        /// <summary>
        /// Pointer to the next scope
        /// </summary>
        public Scope? Next { get; protected set; }

        #endregion


        #region ICollection

        /// <summary>
        /// Count of all registered contracts
        /// </summary>
        public int Count => Index;

        public bool IsSynchronized => false;

        object ICollection.SyncRoot => SyncRoot;

        #endregion


        #region Add

        public abstract void Add(Type type, RegistrationManager manager);

        public abstract RegistrationManager? Add(Type type, string name, RegistrationManager manager);

        public abstract RegistrationManager? Add(in Contract contract, RegistrationManager manager);

        /// <summary>
        /// Set <see cref="Contract"/> entries for registrations
        /// </summary>
        /// <param name="data"><see cref="ReadOnlySpan{RegistrationDescriptor}"/> of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void Add(in ReadOnlySpan<RegistrationDescriptor> span);

        #endregion


        #region Get

        /// <summary>
        /// Get registration data for the contract
        /// </summary>
        /// <param name="contract"><see cref="Contract"/> to look for</param>
        /// <returns>Returns <see cref="RegistrationManager"/> or null if nothing found</returns>
        public abstract RegistrationManager? Get(in Contract contract);

        /// <summary>
        /// Search for <see cref="Contract"/> holding generic type definition and create constructible
        /// registration if found.
        /// </summary>
        /// <param name="contract">Constructible <see cref="Contract"/></param>
        /// <param name="factory"><see cref="Contract"/> of the generic factory</param>
        /// <returns>New <see cref="RegistrationManager"/> created from factory manager if found or null</returns>
        public abstract RegistrationManager? Get(in Contract contract, in Contract factory);

        #endregion


        #region Contains

        /// <summary>
        /// Determines whether the <see cref="Scope"/> contains a specific <see cref="Contract"/>
        /// </summary>
        /// <param name="contract">The <see cref="Type"/> and the Name to look for</param>
        /// <returns>True if <see cref="Contract"/> is found</returns>
        public abstract bool Contains(in Contract contract);

        #endregion


        #region Hierarchy

        /// <summary>
        /// Creates child scope
        /// </summary>
        /// <param name="capacity">Preallocated capacity</param>
        /// <returns>New child scope</returns>
        public abstract Scope CreateChildScope(int capacity);

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Disposable

        /// <summary>
        /// Collection of <see cref="IDisposable"/> objects that this scope owns
        /// </summary>
        public ICollection<IDisposable> Disposables { get; }

        #endregion
    }
}
