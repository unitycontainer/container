using System;
using System.Collections.Generic;
using Unity.Container;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Properties

        public string? Name { get; }

        public UnityContainer Root { get; }

        public UnityContainer? Parent { get; }

        #endregion


        #region Constructors

        /// <summary>
        /// Creates container with name 'root' and allocates 37 slots for contracts
        /// </summary>
        public UnityContainer() : this(Defaults.DEFAULT_ROOT_NAME, Defaults.DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container and allocates 37 slots for contracts
        /// </summary>
        /// <param name="name">Name of the container</param>
        public UnityContainer(string name) : this(name, Defaults.DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container with name 'root'
        /// </summary>
        /// <param name="capacity">Preallocated capacity</param>
        public UnityContainer(int capacity) : this(Defaults.DEFAULT_ROOT_NAME, capacity)
        { }

        #endregion


        #region Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name) 
            => _scope.Contains(new Contract(type, name));

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                var enumerator = _scope.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    yield return enumerator.Registration;
                }
            }
        }

        #endregion


        #region Child Containers

        /// <summary>
        /// Creates a child container with given name
        /// </summary>
        /// <param name="name">Name of the child container</param>
        /// <returns>Instance of child <see cref="UnityContainer"/> container</returns>
        private UnityContainer CreateChildContainer(string? name, int capacity)
        {
            // Create child container
            var container = new UnityContainer(this, name, capacity);

            // Add to lifetime manager
            _scope.Disposables.Add(container);

            // Raise event if required
            _childContainerCreated?.Invoke(this, container._context = new PrivateExtensionContext(container));

            return container;
        }

        #endregion
    }
}
