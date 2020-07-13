using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unity.Container
{
    [DebuggerDisplay("Size = { Count }, Version = { Version }", Name = "Scope({ Container.Name })")]
    public abstract partial class Scope
    {
        /// <summary>
        /// Owner container
        /// </summary>
        public UnityContainer Container => _container;

        /// <summary>
        /// Version of scope
        /// </summary>
        /// <remarks>
        /// Scope version is increased every time registrations change
        /// in any way
        /// </remarks>
        public int Version => _version;

        /// <summary>
        /// Registration count
        /// </summary>
        public virtual int Count { get; }

        /// <summary>
        /// Collection of <see cref="IDisposable"/> objects that this scope owns
        /// </summary>
        public ICollection<IDisposable> Disposables => _disposables;

        public abstract IEnumerable<ContainerRegistration> Registrations { get; }

        public abstract void Register(ref RegistrationData data);

        public abstract bool IsRegistered(Type type);

        public abstract bool IsRegistered(Type type, string name);


        /// <summary>
        /// Creates new scope for child container
        /// </summary>
        /// <param name="container"><see cref="UnityContainer"/> that owns the scope</param>
        /// <returns>New scope associated with the container</returns>
        public abstract Scope CreateChildScope(UnityContainer container);
    }
}
