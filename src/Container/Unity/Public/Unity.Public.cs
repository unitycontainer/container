using Unity.Container;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constants

        /// <summary>
        /// Default name of root container
        /// </summary>
        private const string DEFAULT_ROOT_NAME = "root";

        /// <summary>
        /// Default capacity of root container
        /// </summary>
        private const int DEFAULT_ROOT_CAPACITY = 59;

        #endregion


        #region Properties

        public string? Name { get; }

        public UnityContainer Root { get; }

        public UnityContainer? Parent { get; }

        #endregion


        #region Constructors

        /// <summary>
        /// Creates container with name 'root' and allocates 37 slots for contracts
        /// </summary>
        public UnityContainer() : this(DEFAULT_ROOT_NAME, DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container and allocates 37 slots for contracts
        /// </summary>
        /// <param name="name">Name of the container</param>
        public UnityContainer(string name) : this(name, DEFAULT_ROOT_CAPACITY)
        { }

        /// <summary>
        /// Creates container with name 'root'
        /// </summary>
        /// <param name="capacity">Pre-allocated capacity</param>
        public UnityContainer(int capacity) : this(DEFAULT_ROOT_NAME, capacity)
        { }

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
            Scope.Add(new WeakDisposable(container));

            // Raise event if required
            _childContainerCreated?.Invoke(this, container._context = new PrivateExtensionContext(container));

            return container;
        }

        #endregion
    }
}
