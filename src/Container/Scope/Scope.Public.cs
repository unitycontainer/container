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
        public int Version => _version;

        /// <summary>
        /// Contract names
        /// </summary>
        public virtual int Names => _namesCount;

        #endregion


        #region Hierarchy

        /// <summary>
        /// Creates child scope
        /// </summary>
        /// <returns>New child scope</returns>
        public abstract Scope CreateChildScope();

        #endregion


        #region Add

        /// <summary>
        /// Add <see cref="Contract"/> entries for specified types
        /// </summary>
        /// <param name="manager"><see cref="RegistrationManager"/> containing the registration</param>
        /// <param name="registerAs">Collection of <see cref="Type"/> aliases</param>
        public abstract void Add(RegistrationManager manager, params Type[] registerAs);

        /// <summary>
        /// Add <see cref="Contract"/> entries for registrations
        /// </summary>
        /// <param name="data"><see cref="ReadOnlySpan{RegistrationDescriptor}"/> of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void Add(in ReadOnlySpan<RegistrationDescriptor> span);

        /// <summary>
        /// Add <see cref="Contract"/> entries for registrations
        /// </summary>
        /// <param name="memory"><see cref="ReadOnlyMemory{T}"/> of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void Add(in ReadOnlyMemory<RegistrationDescriptor> memory);

        #endregion


        #region Contains

        /// <summary>
        /// Determines whether the <see cref="Scope"/> contains a specific anonymous contract
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the anonymous contract</param>
        /// <returns>True if <see cref="Contract"/> is found</returns>
        public abstract bool Contains(Type type);

        /// <summary>
        /// Determines whether the <see cref="Scope"/> contains a specific <see cref="Contract"/>
        /// </summary>
        /// <param name="type"><see cref="Type"/> of the <see cref="Contract"/></param>
        /// <param name="name">Name of the <see cref="Contract"/></param>
        /// <returns>True if <see cref="Contract"/> is found</returns>
        public abstract bool Contains(Type type, string name);

        #endregion


        /// <summary>
        /// Collection of <see cref="IDisposable"/> objects that this scope owns
        /// </summary>
        public ICollection<IDisposable> Disposables => _disposables;



        // TODO: Replace with structure
        public virtual IEnumerable<ContainerRegistration> GetRegistrations 
            => throw new NotImplementedException();

    }
}
