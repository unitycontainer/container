using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("{GetType().Name,nq}: Contracts = { Count }, Version = { Version }")]
    public abstract partial class Scope
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
        /// Count of registered contracts
        /// </summary>
        public int Count => Index;

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


        #region Hierarchy

        /// <summary>
        /// Creates child scope
        /// </summary>
        /// <param name="capacity">Preallocated capacity</param>
        /// <returns>New child scope</returns>
        public abstract Scope CreateChildScope(int capacity);

        #endregion


        #region Add

        /// <summary>
        /// Set <see cref="Contract"/> entries for registrations
        /// </summary>
        /// <param name="data"><see cref="ReadOnlySpan{RegistrationDescriptor}"/> of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void Add(in ReadOnlySpan<RegistrationDescriptor> span);

        /// <summary>
        /// Set <see cref="Contract"/> entries for registrations
        /// </summary>
        /// <param name="state">Array of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void AddAsync(object? state);

        #endregion


        #region Internal set

        /// <summary>
        /// Set internal <see cref="Contract"/> entries
        /// </summary>
        /// <remarks>
        /// This method does not check if sufficient space is available, it assumes enough slots 
        /// are preallocated. This method does not increase <see cref="Version"/> assuming all 
        /// added contracts are built in.
        /// </remarks>
        /// <param name="manager"><see cref="RegistrationManager"/> containing the registration</param>
        /// <param name="registerAs">Collection of <see cref="Type"/> aliases</param>
        internal abstract void SetInternal(RegistrationManager manager, params Type[] registerAs);

        #endregion


        #region Contains

        /// <summary>
        /// Determines whether the <see cref="Scope"/> contains a specific <see cref="Contract"/>
        /// </summary>
        /// <param name="contract">The <see cref="Type"/> and the Name to look for</param>
        /// <returns>True if <see cref="Contract"/> is found</returns>
        public abstract bool Contains(in Contract contract);

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


        #region Disposable

        /// <summary>
        /// Collection of <see cref="IDisposable"/> objects that this scope owns
        /// </summary>
        public ICollection<IDisposable> Disposables { get; }
        
        #endregion
    }
}
