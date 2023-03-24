using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
        private WeakReference<UnityRegistrations>? _cache;

        #endregion


        #region Properties

        /// <inheritdoc />
        [DebuggerBrowsable(DebuggerBrowsableState.Never)] 
        IUnityContainer? IUnityContainer.Parent => Parent;

        #endregion


        #region Registration

        /// <inheritdoc />
        public IUnityContainer RegisterType(Type? contractType, Type implementationType, string? contractName, 
            ITypeLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            var type = implementationType ?? contractType ?? throw new ArgumentNullException(nameof(implementationType));

            var manager = (LifetimeManager)(lifetimeManager ?? LifetimeManager.DefaultTypeLifetime);

            if (RegistrationCategory.Uninitialized != manager.Category)
                manager = manager.Clone();

            if (null != injectionMembers && 0 != injectionMembers.Length)
                manager.Inject(injectionMembers);

            manager.Category = RegistrationCategory.Type;
            manager.Data = type;

            // Register the manager
            var scope = manager is SingletonLifetimeManager
                      ? Root.Scope
                      : Scope;

            scope.Register(contractType ?? implementationType!, contractName, manager);

            return this;
        }

        public IUnityContainer RegisterInstance(Type? contractType, string? contractName, object? instance, 
            IInstanceLifetimeManager? lifetimeManager)
        {
            var type = contractType ?? instance?.GetType() ?? 
                throw new ArgumentNullException(nameof(contractType), "Contract Type must be provided when instance is 'null'");

            var manager = (LifetimeManager)(lifetimeManager ?? LifetimeManager.DefaultInstanceLifetime);

            if (RegistrationCategory.Uninitialized != manager.Category)
                manager = manager.Clone();

            manager.Category = RegistrationCategory.Instance;
            manager.Data = instance;
            manager.SetPipeline(Policies.InstancePipeline);

            // Register the manager
            var scope = manager is SingletonLifetimeManager
                      ? Root.Scope
                      : Scope;

            scope.Register(type, contractName, manager);

            // Add IDisposable
            if (instance is IDisposable disposable) scope.Add(disposable);

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer RegisterFactory(Type contractType, string? contractName, IUnityContainer.FactoryDelegate factory,
            IFactoryLifetimeManager? lifetimeManager)
        {
            var manager = (LifetimeManager)(lifetimeManager ?? LifetimeManager.DefaultFactoryLifetime);

            if (RegistrationCategory.Uninitialized != manager.Category)
                manager = manager.Clone();

            manager.Category = RegistrationCategory.Factory;
            manager.Data = factory ?? throw new ArgumentNullException(nameof(factory));
            manager.SetPipeline(Policies.FactoryPipeline);

            // Register the manager
            var scope = manager is SingletonLifetimeManager
                      ? Root.Scope
                      : Scope;

            scope.Register(contractType ?? throw new ArgumentNullException(nameof(contractType)), 
                           contractName, manager);

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer Register(params RegistrationDescriptor[] descriptors)
        {
            ReadOnlySpan<RegistrationDescriptor> span = descriptors;

            // Register with the scope
            Scope.Register(in span);

            // Report registration
            _registering?.Invoke(this, in span);

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer Register(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            // Register with the scope
            Scope.Register(in span);

            // Report registration
            _registering?.Invoke(this, in span);

            return this;
        }

        #endregion


        #region Registrations collection

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations 
            => null != _cache && _cache.TryGetTarget(out var cache) && Scope.Version == cache.Version()
                    ? cache : new UnityRegistrations(this, Scope);


        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            var contract = new Contract(type, name);
            return Scope.Contains(in contract);
        }

        #endregion


        #region Resolution

        /// <inheritdoc />
        [SkipLocalsInit]
        public object? Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            Contract contract = new Contract(type, name);
            RegistrationManager? manager;

            // Look for registration
            if (null != (manager = Scope.Get(in contract)))
            {
                //Registration found, check value
                var value = manager.GetValue(Scope);
                if (!ReferenceEquals(NoValue, value)) return value;

                // Resolve registration
                return RegisteredThrowing(ref contract, manager, overrides);
            }

            // Resolve 
            return UnregisteredThrowing(ref contract, overrides);
        }


        /// <inheritdoc />
        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            BuilderContext context;

            var contract = new Contract(type, name);
            var request = BuilderContext.NewRequest(overrides);

            context = request.Context(this, ref contract);
            context.Existing = existing;
            // TODO: BuildUp 
            context.Existing = Resolve(ref context);

            //if (request.IsFaulted) throw new ResolutionFailedException(ref context);

            return context.Existing!;
        }

        #endregion


        #region Child Container

        /// <summary>
        /// Creates a child container with given name
        /// </summary>
        /// <param name="name">Name of the child container</param>
        /// <returns>Instance of child <see cref="UnityContainer"/> container</returns>
        IUnityContainer IUnityContainer.CreateChildContainer(string? name, int capacity)
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
