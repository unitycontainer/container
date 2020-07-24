using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("{GetType().Name,nq}: Contracts = { Contracts }, Names = { Names }, Version = { Version }")]
    public abstract partial class Scope
    {
        #region Hierarchy

        /// <summary>
        /// Reference to parent scope
        /// </summary>
        public Scope? Next => _next;

        /// <summary>
        /// Creates child scope
        /// </summary>
        /// <returns>New child scope</returns>
        public abstract Scope CreateChildScope();

        #endregion


        #region Quantitative Members

        /// <summary>
        /// Version of scope
        /// </summary>
        /// <remarks>
        /// Scope version is increased every time registrations changes
        /// </remarks>
        public int Version => _version;

        /// <summary>
        /// Registration count
        /// </summary>
        public abstract int Contracts { get; }

        /// <summary>
        /// Contract names
        /// </summary>
        public abstract int Names { get; }

        #endregion


        #region Add

        /// <summary>
        /// Add <see cref="Contract"/> entries for specified types
        /// </summary>
        /// <param name="manager"><see cref="RegistrationManager"/> containing the registration</param>
        /// <param name="registerAs">Collection of <see cref="Type"/> aliases</param>
        public abstract void Add(RegistrationManager manager, params Type[] registerAs);

        /// <summary>
        /// Add <see cref="Contract"/> entries for registration
        /// </summary>
        /// <param name="data"><see cref="ReadOnlySpan{RegistrationDescriptor}"/> of
        /// <see cref="RegistrationDescriptor"/> structures</param>
        public abstract void Add(in ReadOnlySpan<RegistrationDescriptor> data);

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
        public virtual IEnumerable<ContainerRegistration> GetRegistrations { get; }

    }
}
