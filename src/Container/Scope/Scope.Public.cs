using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("{GetType().Name,nq}: Contracts = { Contracts }, Names = { Names }, Version = { Version }")]
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
            ? _version : Next.Version + _version;

        /// <summary>
        /// Count of registered <see cref="Contract"/> names
        /// </summary>
        public virtual int Names => _namesCount;

        /// <summary>
        /// Count of registered <see cref="Contract"/> types
        /// </summary>
        public int Contracts => _contractCount;

        /// <summary>
        /// Return <see cref="ReadOnlyMemory{ContainerRegistration}"/> encapsulating
        /// all registered contracts.
        /// </summary>
        public ReadOnlyMemory<ContainerRegistration> Memory 
            => new ReadOnlyMemory<ContainerRegistration>(_contractData, 1, _contractCount);

        /// <summary>
        /// Storage capacity of the scope
        /// </summary>
        public int Capacity => _contractData.Length;

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
        /// Add internal <see cref="Contract"/> entries
        /// </summary>
        /// <remarks>
        /// This method does not check if sufficient space is available. It assumes enough slots 
        /// are preallocated.
        /// </remarks>
        /// <param name="manager"><see cref="RegistrationManager"/> containing the registration</param>
        /// <param name="registerAs">Collection of <see cref="Type"/> aliases</param>
        internal abstract void Add(RegistrationManager manager, params Type[] registerAs);

        /// <summary>
        /// Add <see cref="Contract"/> entries for registrations
        /// </summary>
        /// <param name="data"><see cref="ReadOnlySpan{RegistrationDescriptor}"/> of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void Add(in ReadOnlySpan<RegistrationDescriptor> span);

        /// <summary>
        /// Add <see cref="Contract"/> entries for registrations
        /// </summary>
        /// <param name="state">Array of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void AddAsync(object? state);

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
        public ICollection<IDisposable> Disposables => _disposables;
        
        #endregion
    }
}
