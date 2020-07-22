using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Lifetime;

namespace Unity.Container
{
    [DebuggerDisplay("Contracts = { Contracts }, Names = { Names }, Version = { Version }")]
    public abstract partial class Scope
    {
        /// <summary>
        /// Parent scope
        /// </summary>
        public Scope? Next => _next;

        /// <summary>
        /// Version of scope
        /// </summary>
        /// <remarks>
        /// Scope version is increased every time registrations changes
        /// </remarks>
        public int Version => _version;


        /// <summary>
        /// Add <see cref="Contract"/> entries for specified types
        /// </summary>
        /// <param name="manager"><see cref="RegistrationManager"/> containing the registration</param>
        /// <param name="registerAs">Collection of <see cref="Type"/> aliases</param>
        public abstract void Add(LifetimeManager manager, params Type[] registerAs);

        /// <summary>
        /// Add <see cref="Contract"/> entries for registration
        /// </summary>
        /// <param name="data"><see cref="RegistrationData"/> registration data</param>
        public abstract void Add(in RegistrationData data);


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












        /// <summary>
        /// Registration count
        /// </summary>
        public abstract int Contracts { get; }

        /// <summary>
        /// Contract names
        /// </summary>
        public abstract int Names { get; }

        /// <summary>
        /// Collection of <see cref="IDisposable"/> objects that this scope owns
        /// </summary>
        public ICollection<IDisposable> Disposables => _disposables;



        // TODO: Replace with structure
        public abstract IEnumerable<ContainerRegistration> Registrations { get; }

        public abstract Scope CreateChildScope();
    }
}
