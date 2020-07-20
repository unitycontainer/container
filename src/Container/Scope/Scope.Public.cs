using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("Contracts = { Contracts }, Names = { Names }, Version = { Version }")]
    public abstract partial class Scope
    {
        /// <summary>
        /// Parent scope
        /// </summary>
        public Scope? Parent => _parent;

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

        /// <summary>
        /// Collection of <see cref="IDisposable"/> objects that this scope owns
        /// </summary>
        public ICollection<IDisposable> Disposables => _disposables;



        // TODO: Replace with structure
        public abstract IEnumerable<ContainerRegistration> Registrations { get; }

        public abstract void Register(in RegistrationData data);

        public abstract bool IsRegistered(Type type);

        public abstract bool IsRegistered(Type type, string name);

        public abstract Scope CreateChildScope();
    }
}
