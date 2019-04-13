using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Extension;
using Unity.Factories;
using Unity.Lifetime;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;
using Unity.Strategies;

namespace Unity
{
    public partial class UnityContainer
    {
        #region Constructors

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
        {
            // Root of the hierarchy
            _root = this;

            // Lifetime Container
            LifetimeContainer = new LifetimeContainer(this);

            // Registry
            InitializeRootRegistry();

            /////////////////////////////////////////////////////////////

            // Registrations
            _registrations = new Registrations(ContainerInitialCapacity);

            // Context
            _context = new ContainerContext(this);

            // Methods
            _get = Get;
            _getGenericRegistration = GetOrAddGeneric;
            IsTypeExplicitlyRegistered = IsTypeTypeExplicitlyRegisteredLocally;

            // Build Strategies
            _strategies = new StagedStrategyChain<BuilderStrategy, UnityBuildStage>
            {
                {   // Array
                    new ArrayResolveStrategy(
                        typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveArray)),
                        typeof(UnityContainer).GetTypeInfo().GetDeclaredMethod(nameof(ResolveGenericArray))),
                    UnityBuildStage.Enumerable
                },
                {new BuildKeyMappingStrategy(), UnityBuildStage.TypeMapping},   // Mapping
                {new LifetimeStrategy(), UnityBuildStage.Lifetime},             // Lifetime
                {new BuildPlanStrategy(), UnityBuildStage.Creation}             // Build
            };

            // Update on change
            _strategies.Invalidated += OnStrategiesChanged;
            _strategiesChain = _strategies.ToArray();


            // Default Policies and Strategies
            SetDefaultPolicies(this);
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        public bool IsRegistered(Type type, string name)
        {
            int hashCode = NamedType.GetHashCode(type, name) & 0x7FFFFFFF;

            // Iterate through hierarchy and check if exists
            for (var container = this; null != container; container = container._parent)
            {
                // Skip to parent if no registry
                if (null == container._registry)
                    continue;

                // Look for exact match
                if (container._registry.Contains(hashCode, type))
                    return true;
            }

            return false;
        }

        /// <inheritdoc />
        public IEnumerable<IContainerRegistration> Registrations => GetExplicitRegistrations(this);

        #endregion


        #region Extension Management

        /// <summary>
        /// Add an extension to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer AddExtension(IUnityContainerExtensionConfigurator extension)
        {
            lock (LifetimeContainer)
            {
                if (null == _extensions)
                    _extensions = new List<IUnityContainerExtensionConfigurator>();

                _extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
            }
            (extension as UnityContainerExtension)?.InitializeExtension(_context);

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
        public object Configure(Type configurationInterface)
        {
#if NETSTANDARD1_0 || NETCOREAPP1_0
            return _extensions?.FirstOrDefault(ex => configurationInterface.GetTypeInfo()
                                                                           .IsAssignableFrom(ex.GetType()
                                                                           .GetTypeInfo()));
#else
            return _extensions?.FirstOrDefault(ex => configurationInterface.IsAssignableFrom(ex.GetType()));
#endif
        }

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// Disposing the container also disposes any child containers,
        /// and disposes any instances whose lifetimes are managed
        /// by the container.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
