using System;
using System.Collections.Generic;
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

        private WeakReference<Metadata[]>? _cache;

        #endregion


        #region Properties

        /// <inheritdoc />
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
            if (contractName is null)
                lock (_scope.SyncRoot)
                {
                    _scope.Add(contractType ?? implementationType!, manager);
                }
            else
            {
                RegistrationManager? registration;
                lock (_scope.SyncRoot)
                {
                    registration = _scope.Add(contractType ?? implementationType!, contractName, manager);
                }
                if (null != registration) DisposeManager(registration);
            }

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
            if (contractName is null)
            {
                lock (_scope.SyncRoot)
                { 
                    _scope.Add(type, manager);
                }
            }
            else
            {
                RegistrationManager? registration;
                lock (_scope.SyncRoot)
                {
                    registration = _scope.Add(type, contractName, manager);
                }
                if (null != registration) DisposeManager(registration);
            }

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer RegisterFactory(Type contractType, string? contractName, Func<IUnityContainer, Type, string?, ResolverOverride[], object?> factory,
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
            manager.Data = factory;

            // Register the manager
            if (contractName is null)
            {
                lock (_scope.SyncRoot)
                {
                    _scope.Add(contractType, manager);
                }
            }
            else
            {
                RegistrationManager? registration;
                lock (_scope.SyncRoot)
                {
                    registration = _scope.Add(contractType, contractName, manager);
                }
                if (null != registration) DisposeManager(registration);
            }

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer Register(params RegistrationDescriptor[] descriptors)
        {
            ReadOnlySpan<RegistrationDescriptor> span = descriptors;

            // Register with the scope
            lock (_scope.SyncRoot)
            {
                _scope.Add(in span);
            }

            // Report registration
            _registering?.Invoke(this, in span);

            return this;
        }

        /// <inheritdoc />
        public IUnityContainer Register(in ReadOnlySpan<RegistrationDescriptor> span)
        {
            // Register with the scope
            lock (_scope.SyncRoot)
            {
                _scope.Add(in span);
            }

            // Report registration
            _registering?.Invoke(this, in span);

            return this;
        }

        #endregion


        #region Registrations collection

        /// <inheritdoc />
        public IEnumerable<ContainerRegistration> Registrations
        {
            get
            {
                if (null != _cache && _cache.TryGetTarget(out var recording) && _scope.Version == recording.Version())
                {
                    var count = recording.Count();
                    for (var i = 1; i <= count; i++) yield return _scope[in recording[i]].Registration;
                }
                else
                {
                    var set = new RegistrationSet(_scope);
                    var enumerator = new Enumerator(this);

                    while (enumerator.MoveNext())
                    {
                        var manager = enumerator.Manager;

                        if (manager is null || RegistrationCategory.Internal > manager.Category ||
                            !set.Add(in enumerator)) continue;

                        yield return enumerator.Registration;
                    }

                    _cache = new WeakReference<Metadata[]>(set.GetRecording());
                }
            }
        }

        /// <inheritdoc />
        public bool IsRegistered(Type type, string? name)
        {
            var contract = new Contract(type, name);
            return _scope.Contains(in contract);
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
            if (null != (manager = _scope.Get(in contract)))
            {
                //Registration found, check value
                var value = manager.GetValue(_scope);
                if (!ReferenceEquals(RegistrationManager.NoValue, value)) return value;

                // Resolve registration
                return ResolveRegistration(ref contract, manager, overrides);
            }

            // Resolve 
            return ResolveUnregistered(ref contract, overrides);
        }


        /// <inheritdoc />
        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            // TODO: Optimize
            PipelineContext context;

            var contract = new Contract(type, name);
            var request = new RequestInfo(overrides);
            var manager = _scope.Get(in contract);


            // Look for registration
            if (null != manager)
            {
                // Resolve registration
                context = new PipelineContext(this, ref contract, manager, ref request);
                context.Target = existing;

                BuildUpRegistration(ref context);

                //if (request.IsFaulted) throw new ResolutionFailedException(ref context);

            }

            context = new PipelineContext(this, ref contract, ref request);
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
