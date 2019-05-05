using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Builder;
using Unity.Events;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Fields

        const string LifetimeManagerInUse = "The lifetime manager is already registered. WithLifetime managers cannot be reused, please create a new one.";
        private Action<Type, Type>? TypeValidator;

        #endregion


        #region Type Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterType(Type typeFrom, Type typeTo, string name, ITypeLifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            var mappedToType = typeTo;
            var registeredType = typeFrom ?? typeTo;

            // Validate input
            if (null == registeredType) throw new InvalidOperationException($"At least one of Type arguments '{nameof(typeFrom)}' or '{nameof(typeTo)}' must be not 'null'");

            try
            {
                // Lifetime Manager
                var manager = lifetimeManager as LifetimeManager ?? Context.TypeLifetimeManager.CreateLifetimePolicy();
                if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);
                manager.InUse = true;

                // Validate if they are assignable
                TypeValidator?.Invoke(typeFrom, typeTo);

                // Create registration and add to appropriate storage
                var container = manager is SingletonLifetimeManager ? _root : this;
                var registration = new ExplicitRegistration(container, name, _validators, typeTo, manager, injectionMembers);

                // If Disposable add to container's lifetime
                if (manager is IDisposable disposableManager)
                    container.LifetimeContainer.Add(disposableManager);

                // Add or replace existing 
                var previous = container.Register(registeredType, name, registration);

                // Allow reference adjustment and disposal
                if (null != previous && 0 == previous.Release()
                    && previous.LifetimeManager is IDisposable disposable)
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
                        member.AddPolicies<BuilderContext, ExplicitRegistration>(
                            registeredType, mappedToType, name, ref registration);
                    }
                }

                // Check what strategies to run
                registration.Processors = Context.TypePipelineCache;

                // Raise event
                container.Registering?.Invoke(this, new RegisterEventArgs(registeredType,
                                                                          mappedToType,
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
                if (null != lifetimeManager && !(lifetimeManager is TransientLifetimeManager)) parts.Add(lifetimeManager.ToString());
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
            if (null == registeredType) throw new InvalidOperationException($"At least one of Type arguments '{nameof(type)}' or '{nameof(instance)}' must be not 'null'");

            try
            {
                // Lifetime Manager
                var manager = lifetimeManager as LifetimeManager ?? Context.InstanceLifetimeManager.CreateLifetimePolicy();
                if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);

                manager.InUse = true;
                manager.SetValue(instance, LifetimeContainer);

                // Create registration and add to appropriate storage
                var container = manager is SingletonLifetimeManager ? _root : this;
                var registration = new ExplicitRegistration(container, name, null, mappedToType ?? registeredType, manager);

                // If Disposable add to container's lifetime
                if (manager is IDisposable disposableManager)
                    container.LifetimeContainer.Add(disposableManager);

                // Register type
                var previous = container.Register(registeredType, name, registration);

                // Allow reference adjustment and disposal
                if (null != previous && 0 == previous.Release()
                    && previous.LifetimeManager is IDisposable disposable)
                {
                    // Dispose replaced lifetime manager
                    container.LifetimeContainer.Remove(disposable);
                    disposable.Dispose();
                }

                // Check what strategies to run
                registration.Processors = Context.InstancePipelineCache;

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
            var manager = lifetimeManager as LifetimeManager ?? Context.FactoryLifetimeManager.CreateLifetimePolicy();
            if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);
            manager.InUse = true;

            // Create registration and add to appropriate storage
            var container = manager is SingletonLifetimeManager ? _root : this;
            var registration = new ExplicitRegistration(container, name, _validators, type, manager);

            registration.Set(typeof(ResolveDelegate<BuilderContext>), 
                            (ResolveDelegate<BuilderContext>)((ref BuilderContext c) => factory(c.Container, c.Type, c.Name)));

            // Add or replace existing 
            var previous = container.Register(type, name, registration);

            // Allow reference adjustment and disposal
            if (null != previous && 0 == previous.Release()
                && previous.LifetimeManager is IDisposable disposable)
            {
                // Dispose replaced lifetime manager
                container.LifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // If Disposable add to container's lifetime
            if (manager is IDisposable managerDisposable)
                container.LifetimeContainer.Add(managerDisposable);

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
        object? IUnityContainer.Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            // Setup Context
            var context = new BuilderContext
            {
                List             = new PolicyList(),
                Type             = type,
                Name             = name,
                ContainerContext = Context,
                Registration     = GetRegistration(type ?? throw new ArgumentNullException(nameof(type)), name),
                Overrides        = null != overrides && 0 < overrides.Length ? overrides : null,
                ResolvePlan      = ContextResolvePlan,
            };

            // Create an object
            return ExecutePipeline(ref context);
        }

        #endregion


        #region BuildUp existing object

        /// <inheritdoc />
        public object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides)
        {
            // Setup Context
            var context = new BuilderContext
            {
                List             = new PolicyList(),
                Type             = type,
                Name             = name,
                Existing         = existing ?? throw new ArgumentNullException(nameof(existing)),
                ContainerContext = Context,
                Registration     = GetRegistration(type ?? throw new ArgumentNullException(nameof(type)), name),
                Overrides        = null != overrides && 0 < overrides.Length ? overrides : null,
                ResolvePlan      = ContextResolvePlan,
            };

            // Validate if they are assignable
            TypeValidator?.Invoke(type, existing.GetType());

            // Initialize an object
            return ExecutePipeline(ref context);
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
