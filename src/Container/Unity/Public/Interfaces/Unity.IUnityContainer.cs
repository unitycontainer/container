using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Container;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

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

            // Validate and initialize registration manager
            var manager = (lifetimeManager ?? DefaultTypeLifetimeManager(type)) as LifetimeManager ?? 
                throw new ArgumentException("Invalid Lifetime Manager", nameof(lifetimeManager));

            if (RegistrationCategory.Uninitialized != manager.Category)
                manager = manager.Clone();

            if (null != injectionMembers && 0 != injectionMembers.Length)
                manager.Add(injectionMembers);

            manager.Category = RegistrationCategory.Type;
            manager.Data = type;

            // Register the manager
            Scope.Register(contractType ?? implementationType!, contractName, manager);

            return this;
        }

        public IUnityContainer RegisterInstance(Type? contractType, string? contractName, object? instance, 
            IInstanceLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            var type = contractType ?? instance?.GetType() ?? 
                throw new ArgumentNullException("Contract Type must be provided when instance is 'null'", nameof(contractType));

            // Validate and initialize registration manager
            var manager = (lifetimeManager ?? DefaultInstanceLifetimeManager(type)) as LifetimeManager ??
                throw new ArgumentException("Invalid Lifetime Manager", nameof(lifetimeManager));

            if (RegistrationCategory.Uninitialized != manager.Category)
                manager = manager.Clone();

            if (null != injectionMembers && 0 != injectionMembers.Length)
                manager.Add(injectionMembers);

            manager.Category = RegistrationCategory.Instance;
            manager.Data = instance;

            // Register the manager
            Scope.Register(type, contractName, manager);

            // Add IDisposable
            if (instance is IDisposable disposable) Scope.Add(disposable);

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer RegisterFactory(Type contractType, string? contractName, IUnityContainer.FactoryDelegate factory,
            IFactoryLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            // Validate and initialize registration manager
            var manager = (lifetimeManager ?? DefaultFactoryLifetimeManager(contractType)) as LifetimeManager ??
                throw new ArgumentException("Invalid Lifetime Manager", nameof(lifetimeManager));

            if (RegistrationCategory.Uninitialized != manager.Category)
                manager = manager.Clone();

            if (null != injectionMembers && 0 != injectionMembers.Length)
                manager.Add(injectionMembers);

            manager.Category = RegistrationCategory.Factory;
            manager.Data = factory ?? throw new ArgumentNullException(nameof(factory));

            // Register the manager
            Scope.Register(contractType, contractName, manager);

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
                return ResolveRegistered(ref contract, manager, overrides);
            }

            // Resolve 
            return ResolveUnregistered(ref contract, overrides);
        }


        /// <inheritdoc />
        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            // TODO: Optimize
            BuilderContext context;

            var contract = new Contract(type, name);
            var request = new RequestInfo(overrides);
            var manager = Scope.Get(in contract);


            // Look for registration
            if (null != manager)
            {
                // Resolve registration
                context = new BuilderContext(this, ref contract, manager, ref request);
                context.Target = existing;

                BuildUpRegistration(ref context);

                //if (request.IsFaulted) throw new ResolutionFailedException(ref context);

            }

            context = new BuilderContext(this, ref contract, ref request);
            context.Target = existing;
            // TODO: BuildUp 
            context.Target = Resolve(ref context);

            //if (request.IsFaulted) throw new ResolutionFailedException(ref context);

            return context.Target!;
        }

        #endregion


        #region Child Container

        /// <inheritdoc />
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IUnityContainer IUnityContainer.CreateChildContainer(string? name, int capacity)
            => CreateChildContainer(name, capacity);

        #endregion
    }
}
