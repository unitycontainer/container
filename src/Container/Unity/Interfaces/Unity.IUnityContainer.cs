﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
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
            if (null == contractName)
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
            if (null == contractName)
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
        public IUnityContainer RegisterFactory(Type contractType, string? contractName, Func<IUnityContainer, Type, string?, ResolverOverride[], object> factory,
            IFactoryLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers)
        {
            // Validate and initialize registration manager
            var manager = (lifetimeManager ?? DefaultFactoryLifetimeManager(contractType)) as LifetimeManager ??
                throw new ArgumentException("Invalid Lifetime Manager", nameof(lifetimeManager));

            if (RegistrationCategory.Uninitialized != manager.Category)
                manager = manager.Clone();

            if (null != injectionMembers && 0 != injectionMembers.Length)
                manager.Add(injectionMembers);

            manager.Category = RegistrationCategory.Instance;
            manager.Data = factory;

            // Register the manager
            if (null == contractName)
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


        #region Resolution

        /// <inheritdoc />
        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            throw new NotImplementedException();
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