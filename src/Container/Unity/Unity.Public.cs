using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Container;
using Unity.Extension;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Properties

        public string? Name { get; }

        public UnityContainer Root { get; }

        public UnityContainer? Parent { get; }

        #endregion


        #region Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            var scope = _scope;

            do
            {
                if (_scope.Contains(type, name))
                    return true;
            }
            while (null != (scope = scope.Next));

            return false;
        }


        public UnityContainer Register(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            // Register with the scope
            _scope.Add(in span);

            // Report registration
            _registering?.Invoke(this, in span);

            return this;
        }

        public async ValueTask RegisterAsync(params RegistrationDescriptor[] memory)
        {
            await Task.Run(() => Register(memory));
        }


        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                var scope = _scope;
                var levels = new List<Scope>(_level) { _scope };

                while (null != (scope = scope.Next))
                {
                    if (scope.Contracts > DEFAULT_CONTRACTS)
                        levels.Add(scope);
                }

                return 1 == levels.Count
                    ? (IEnumerable<ContainerRegistration>)new SingleScopeEnumerator(this)
                    : new MultiScopeEnumerator(levels);
            }
        }

        #endregion


        #region Extension Management

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public UnityContainer AddExtension(UnityContainerExtension extension)
        {
            if (null == _context)
            { 
                lock (_scope)
                {
                    if (null == _context) _context = new PrivateExtensionContext(this);
                }
            }

            lock (_context)
            { 
                if (extension is IUnityContainerExtensionConfigurator configurator)
                { 
                    (_extensions ??= new List<IUnityContainerExtensionConfigurator>())
                        .Add(configurator);
                }

                extension?.InitializeExtension(_context);
            }

            return this;
        }

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="method"><see cref="Action{ExtensionContext}"/> delegate</param>
        /// <returns>The <see cref="UnityContainer"/> that is being extended</returns>
        public UnityContainer AddExtension(Action<ExtensionContext> method)
        {
            if (null == _context)
            {
                lock (_scope)
                {
                    if (null == _context) _context = new PrivateExtensionContext(this);
                }
            }

            lock (_context)
            { 
                method?.Invoke(_context ??= new PrivateExtensionContext(this));
            }

            return this;
        }

        /// <summary>
        /// Resolve access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public object? Configure(Type configurationInterface)
        {
            return _extensions?.FirstOrDefault(ex => configurationInterface.IsAssignableFrom(ex.GetType()));
        }

        #endregion


        #region Child Containers

        /// <summary>
        /// Creates a child container with given name
        /// </summary>
        /// <param name="name">Name of the child container</param>
        /// <returns>Instance of child <see cref="UnityContainer"/> container</returns>
        private UnityContainer CreateChildContainer(string? name = null)
        {
            // Create child container
            var container = new UnityContainer(this, name);

            // Add to lifetime manager
            _scope.Disposables.Add(container);

            // Raise event if required
            _childContainerCreated?.Invoke(this, container._context = new PrivateExtensionContext(container));

            return container;
        }

        #endregion

    }
}
