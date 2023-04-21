namespace Unity
{
    public sealed partial class UnityContainer
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
    }
}
