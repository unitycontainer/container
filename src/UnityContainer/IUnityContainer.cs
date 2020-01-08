using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using Unity.Events;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Type Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterType(Type typeFrom, Type? typeTo, string? name, ITypeLifetimeManager? lifetimeManager, InjectionMember[] injectionMembers)
        {
            // Validate input
            var registeredType = ValidateType(typeFrom, typeTo);

            try
            {
                // Lifetime Manager
                var manager = lifetimeManager as LifetimeManager ?? Context.TypeLifetimeManager.CreateLifetimePolicy();
                if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);
                manager.InUse = true;

                // Create registration and add to appropriate storage
                var container = manager is SingletonLifetimeManager ? _root : this;

                // If Disposable add to container's lifetime
                if (manager is IDisposable disposableManager)
                    container.LifetimeContainer.Add(disposableManager);

                // Add or replace existing 
                var registration = new ExplicitRegistration(container, name, typeTo, manager, injectionMembers);
                var previous = container.RegisterType(registeredType, name, registration);

                // Allow reference adjustment and disposal
                if (null != previous && previous is IDisposable disposable)
                {
                    // Dispose replaced lifetime manager
                    container.LifetimeContainer.Remove(disposable);
                    disposable.Dispose();
                }

                // Add Injection Members
                if (null != injectionMembers && injectionMembers.Length > 0)
                {
                    foreach (var member in injectionMembers)
                    {
                        member.AddPolicies<PipelineContext, ExplicitRegistration>(
                            registeredType, typeTo, name, ref registration);
                    }
                }

                // Check what strategies to run
                registration.Processors = Context.TypePipelineCache;

                // Raise event
                container.Registering?.Invoke(this, new RegisterEventArgs(registeredType,
                                                                          typeTo,
                                                                          name,
                                                                          manager));
            }
            catch (Exception ex)
            {
                var builder = new StringBuilder();

                builder.AppendLine(ex.Message);
                builder.AppendLine();

                var parts = new List<string>();
                var generics = null == typeFrom ? typeTo?.Name : $"{typeFrom?.Name},{typeTo?.Name}";
                if (null != name) parts.Add($" '{name}'");
                if (null != lifetimeManager && !(lifetimeManager is TransientLifetimeManager)) parts.Add(lifetimeManager.ToString()!);
                if (null != injectionMembers && 0 != injectionMembers.Length)
                    parts.Add(string.Join(" ,", injectionMembers.Select(m => m.ToString())));

                builder.AppendLine($"  Error in:  RegisterType<{generics}>({string.Join(", ", parts)})");
                throw new InvalidOperationException(builder.ToString(), ex);
            }

            return this;
        }

        #endregion


        #region Instance Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterInstance(Type? type, string? name, object? instance, IInstanceLifetimeManager? lifetimeManager)
        {
            var mappedToType = instance?.GetType();
            var registeredType = type ?? mappedToType;

            // Validate input
            if (null == registeredType)
                throw new ArgumentNullException(nameof(type), $"When registering 'null' as an instance, 'Type' is required");

            try
            {
                // Lifetime Manager
                var manager = lifetimeManager as LifetimeManager ?? Context.InstanceLifetimeManager.CreateLifetimePolicy();

                // Root container or local storage
                var container = manager is SingletonLifetimeManager ? _root : this;

                // Set Value
                manager.Set(instance, container.LifetimeContainer);
                manager.PipelineDelegate = (ResolveDelegate<PipelineContext>)((ref PipelineContext c) =>
                {
                    var value = manager.Get(c.LifetimeContainer);
                    return LifetimeManager.NoValue != value 
                                                    ? value
                                                    : throw new ResolutionFailedException(registeredType, name, "Failed to resolve instance");
                });

                // If Disposable register with the container
                if (manager is IDisposable disposableManager) container.LifetimeContainer.Add(disposableManager);

                // Register instance
                var previous = container.RegisterInstance(registeredType, name, manager);

                // Allow reference adjustment and disposal
                if (null != previous && previous is IDisposable disposable)
                {
                    // Dispose replaced lifetime manager
                    container.LifetimeContainer.Remove(disposable);
                    disposable.Dispose();
                }

                // Check what strategies to run
                //registration.Processors = Context.InstancePipelineCache;

                // Raise event
                container.RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(registeredType, instance, name, manager));
            }
            catch (Exception ex)
            {
                var parts = new List<string>();

                if (null != name) parts.Add($" '{name}'");
                var message = $"Error in  RegisterInstance<{registeredType?.Name}>({string.Join(", ", parts)})";
                throw new InvalidOperationException(message, ex);
            }

            return this;
        }

        #endregion


        #region Factory Registration

        /// <inheritdoc />
        public IUnityContainer RegisterFactory(Type type, string? name, Func<IUnityContainer, Type, string?, object?> factory, IFactoryLifetimeManager? lifetimeManager)
        {
            // Validate input
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            // Lifetime Manager
            var manager = lifetimeManager as LifetimeManager ??
                          Context.FactoryLifetimeManager.CreateLifetimePolicy();

            // Target Container
            var container = manager is SingletonLifetimeManager ? _root : this;

            // Create registration
            var registration = new FactoryRegistration(container!, type, name, factory, manager);

            // Register
            var previous = container.RegisterType(type, name, registration);

            // Allow reference adjustment and disposal
            if (null != previous && previous is IDisposable disposable)
            {
                // Dispose replaced lifetime manager
                container.LifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // Check what strategies to run
            registration.Processors = Context.FactoryPipelineCache;

            // Raise event
            container.Registering?.Invoke(this, new RegisterEventArgs(type, type, name, manager));

            // Return Container
            return this;
        }

        #endregion


        #region Getting objects

        /// <inheritdoc />
        [SecuritySafeCritical]
        object? IUnityContainer.Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            // Validate Type
            if (null == type) throw new ArgumentNullException(nameof(type));

            // Get pipeline
            var pipeline = GetPipeline(type, name);

            // Check if already created and acquire a lock if not
            var manager = pipeline.Target as LifetimeManager;
            if (null != manager)
            {
                // Make blocking check for result
                var value = manager.Get(LifetimeContainer);
                if (LifetimeManager.NoValue != value) return value;
            }

            // Setup Context
            var context = new PipelineContext
            {
                List = new PolicyList(),
                Type = type,
                Name = name,
                Overrides = overrides,
                ContainerContext = manager is ContainerControlledLifetimeManager container 
                                 ? (ContainerContext)container.Scope! 
                                 : Context,
            };

            // Execute pipeline
            try
            {
                var value = pipeline(ref context);
                manager?.Set(value, LifetimeContainer);
                return value;
            }
            catch (Exception ex)
            when (ex is InvalidRegistrationException || ex is CircularDependencyException  || ex is ObjectDisposedException)
            {
                var message = CreateErrorMessage(ex);
                throw new ResolutionFailedException(context.Type, context.Name, message, ex);
            }
        }

        #endregion


        #region BuildUp existing object

        /// <inheritdoc />
        [SecuritySafeCritical]
        public object BuildUp(Type? type, object existing, string? name, params ResolverOverride[] overrides)
        {
            var key = new HashKey(type ?? existing?.GetType() ?? throw new ArgumentNullException(nameof(existing)), name);
            var pipeline = GetPipeline(ref key);

            // Setup Context
            var context = new PipelineContext
            {
                Existing = existing ?? throw new ArgumentNullException(nameof(existing)),
                List = new PolicyList(),
                Type = ValidateType(type, existing.GetType()),
                Name = name,
                Overrides = overrides,
                ContainerContext = Context,
            };

            // Initialize an object
            try
            {
                // Execute pipeline
                return pipeline(ref context)!;
            }
            catch (Exception ex)
            when (ex is InvalidRegistrationException || ex is CircularDependencyException || ex is ObjectDisposedException)
            {
                var message = CreateErrorMessage(ex);
                throw new ResolutionFailedException(context.Type, context.Name, message, ex);
            }
        }

        #endregion


        #region Child container management

        /// <inheritdoc />
        IUnityContainer IUnityContainer.CreateChildContainer() => CreateChildContainer();

        /// <inheritdoc />
        IUnityContainer? IUnityContainer.Parent => _parent;

        #endregion
    }
}
