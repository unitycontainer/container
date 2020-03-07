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
        IUnityContainer IUnityContainer.RegisterType(Type registeredType, Type? mappedToType, string? name, ITypeLifetimeManager? lifetimeManager, InjectionMember[] injectionMembers)
        {
            if (null == registeredType) registeredType = mappedToType!;
            if (null == mappedToType) mappedToType = registeredType;
            if (null == registeredType) throw new ArgumentNullException(nameof(registeredType));

            // Validate if they are assignable
            TypeValidator?.Invoke(registeredType, mappedToType);

            try
            {
                // Lifetime Manager
                var manager = null != lifetimeManager 
                            ? (LifetimeManager)lifetimeManager 
                            : (LifetimeManager)TypeLifetimeManager.Clone();

                // Determine storage
                var container = manager is SingletonLifetimeManager ? _root : this;
                manager.Scope = container;

                // Create registration and add to appropriate storage
                var registration = new ContainerRegistration(container, _validators, mappedToType, manager, injectionMembers);

                // Add or replace existing 
                var previous = container.Register(registeredType, name, registration);
                if (previous is ContainerRegistration old &&
                    old.LifetimeManager is IDisposable disposable)
                {
                    // Dispose replaced lifetime manager
                    container.LifetimeContainer.Remove(disposable);
                    disposable.Dispose();
                }

                // If Disposable add to container's lifetime
                if (manager is IDisposable disposableManager)
                    container.LifetimeContainer.Add(disposableManager);

                // Add Injection Members
                if (null != injectionMembers && injectionMembers.Length > 0)
                {
                    foreach (var member in injectionMembers)
                    {
                        member.AddPolicies<BuilderContext, ContainerRegistration>(
                            registeredType, mappedToType, name, ref registration);
                    }
                }

                // Check what strategies to run
                registration.BuildChain = _strategiesChain.ToArray()
                                                          .Where(strategy => strategy.RequiredToBuildType(this,
                                                              registeredType, registration, injectionMembers!))
                                                          .ToArray();
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
                var generics = null == mappedToType ? registeredType?.Name : $"{registeredType?.Name},{mappedToType?.Name}";
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
            var typeFrom = type ?? mappedToType;
            LifetimeManager manager = (null != lifetimeManager)
                                    ? (LifetimeManager)lifetimeManager
                                    : (LifetimeManager)InstanceLifetimeManager.Clone();
            try
            {
                // Validate input
                if (null == typeFrom) throw new InvalidOperationException($"At least one of Type arguments '{nameof(type)}' or '{nameof(instance)}' must be not 'null'");

                manager.SetValue(instance, LifetimeContainer);

                // Create registration and add to appropriate storage
                var container = manager is SingletonLifetimeManager ? _root : this;
                var registration = new ContainerRegistration(container, null, mappedToType!, manager);

                // Add or replace existing 
                var previous = container.Register(typeFrom, name, registration);
                if (previous is ContainerRegistration old &&
                    old.LifetimeManager is IDisposable disposable)
                {
                    // Dispose replaced lifetime manager
                    container.LifetimeContainer.Remove(disposable);
                    disposable.Dispose();
                }

                // If Disposable add to container's lifetime
                if (manager is IDisposable disposableManager)
                    container.LifetimeContainer.Add(disposableManager);

                // Check what strategies to run
                registration.BuildChain = _strategiesChain.ToArray()
                                                          .Where(strategy => strategy.RequiredToResolveInstance(this, registration))
                                                                                     .ToArray();
                // Raise event
                container.RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(typeFrom, instance, name, manager));
            }
            catch (Exception ex)
            {
                var parts = new List<string>();

                if (null != name) parts.Add($" '{name}'");
                if (null != lifetimeManager && !(lifetimeManager is TransientLifetimeManager)) parts.Add(lifetimeManager.ToString()!);

                var message = $"Error in  RegisterInstance<{typeFrom?.Name}>({string.Join(", ", parts)})";
                throw new InvalidOperationException(message, ex);
            }

            return this;
        }

        #endregion


        #region Factory Registration

        /// <inheritdoc />

        public IUnityContainer RegisterFactory(Type type, string? name, Func<IResolveContext, object?> factory, IFactoryLifetimeManager? lifetimeManager, params InjectionMember[] injectionMembers) 
        {
            LifetimeManager manager = (null != lifetimeManager)
                                    ? (LifetimeManager)lifetimeManager
                                    : (LifetimeManager)FactoryLifetimeManager.Clone();
            // Validate input
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            // Create registration and add to appropriate storage
            var container = manager is SingletonLifetimeManager ? _root : this;

            var registration = new ContainerRegistration(container, _validators, type, manager);

            if (manager is PerResolveLifetimeManager)
            {
                ResolveDelegate<BuilderContext> resolveDelegate = (ref BuilderContext context) =>
                {
                    var result = factory(context);
                    var perBuildLifetime = new InternalPerResolveLifetimeManager(result);

                    context.Set(context.Type, context.Name, typeof(LifetimeManager), perBuildLifetime);
                    return result;
                };

                registration.Set(typeof(ResolveDelegate<BuilderContext>), resolveDelegate);
            }
            else
            {
                registration.Set(typeof(ResolveDelegate<BuilderContext>), 
                    (ResolveDelegate<BuilderContext>)((ref BuilderContext context) => factory(context)));
            }

            manager.Scope = container;

            // Add or replace existing 
            var previous = container.Register(type, name, registration);
            if (previous is ContainerRegistration old &&
                old.LifetimeManager is IDisposable disposable)
            {
                // Dispose replaced lifetime manager
                container.LifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // If Disposable add to container's lifetime
            if (manager is IDisposable managerDisposable)
                container.LifetimeContainer.Add(managerDisposable);

            // Check what strategies to run
            registration.BuildChain = _strategiesChain.ToArray()
                                                      .Where(strategy => strategy.RequiredToBuildType(this, type, registration))
                                                      .ToArray();
            // Raise event
            container.Registering?.Invoke(this, new RegisterEventArgs(type, type, name, manager));
            return this;
        }


        public IUnityContainer RegisterFactory(Type type, string? name, ResolveDelegate<IResolveContext> factory, IFactoryLifetimeManager? lifetimeManager) 
        {
            LifetimeManager manager = (null != lifetimeManager)
                                    ? (LifetimeManager)lifetimeManager
                                    : (LifetimeManager)FactoryLifetimeManager.Clone();
            // Validate input
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == factory) throw new ArgumentNullException(nameof(factory));

            // Create registration and add to appropriate storage
            var container = manager is SingletonLifetimeManager ? _root : this;

            var registration = new ContainerRegistration(container, _validators, type, manager);

            if (manager is PerResolveLifetimeManager)
            {
                ResolveDelegate<BuilderContext> resolveDelegate = (ref BuilderContext context) =>
                {
                    // TODO: Casting to interface is wrong
                    IResolveContext c = context;
                    var result = factory(ref c);
                    var perBuildLifetime = new InternalPerResolveLifetimeManager(result);

                    context.Set(context.Type, context.Name, typeof(LifetimeManager), perBuildLifetime);
                    return result;
                };

                registration.Set(typeof(ResolveDelegate<BuilderContext>), resolveDelegate);
            }
            else
            {
                ResolveDelegate<BuilderContext> resolveDelegate = (ref BuilderContext context) =>
                {
                    // TODO: Casting to interface is wrong
                    IResolveContext c = context;
                    return  factory(ref c);
                };

                registration.Set(typeof(ResolveDelegate<BuilderContext>), resolveDelegate);
            }

            manager.Scope = container;

            // Add or replace existing 
            var previous = container.Register(type, name, registration);
            if (previous is ContainerRegistration old &&
                old.LifetimeManager is IDisposable disposable)
            {
                // Dispose replaced lifetime manager
                container.LifetimeContainer.Remove(disposable);
                disposable.Dispose();
            }

            // If Disposable add to container's lifetime
            if (manager is IDisposable managerDisposable)
                container.LifetimeContainer.Add(managerDisposable);

            // Check what strategies to run
            registration.BuildChain = _strategiesChain.ToArray()
                                                      .Where(strategy => strategy.RequiredToBuildType(this, type, registration))
                                                      .ToArray();
            // Raise event
            container.Registering?.Invoke(this, new RegisterEventArgs(type, type, name, manager));
            return this;
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        bool IUnityContainer.IsRegistered(Type type, string? name) => ReferenceEquals(All, name) ? IsTypeExplicitlyRegistered(type)
                                                                                                : _isExplicitlyRegistered(type, name);
        #endregion


        #region Getting objects

        /// <inheritdoc />
        object? IUnityContainer.Resolve(Type type, string? name, params ResolverOverride[] overrides)
        {
            // Verify arguments
            if (null == type) throw new ArgumentNullException(nameof(type));

            var registration = (InternalRegistration)GetRegistration(type, name);
            var context = new BuilderContext
            {
                List = new PolicyList(),
                Lifetime = LifetimeContainer,
                Overrides = null != overrides && 0 == overrides.Length ? null : overrides,
                Registration = registration,
                RegistrationType = type,
                Name = name,
                ExecutePlan = ContextExecutePlan,
                ResolvePlan = ContextResolvePlan,
                Type = registration is ContainerRegistration containerRegistration ? containerRegistration.Type : type,
            };

            return ExecutePlan(ref context);
        }

        #endregion


        #region BuildUp existing object

        /// <inheritdoc />
        object IUnityContainer.BuildUp(Type? type, object existing, string? name, params ResolverOverride[] overrides)
        {
            // Verify arguments
            if (null == existing) throw new ArgumentNullException(nameof(existing));
            if (null == type) type = existing.GetType();

            // Validate if they are assignable
            TypeValidator?.Invoke(type, existing.GetType());

            var registration = (InternalRegistration)GetRegistration(type, name);
            var context = new BuilderContext
            {
                List = new PolicyList(),
                Lifetime = LifetimeContainer,
                Existing = existing,
                Overrides = null != overrides && 0 == overrides.Length ? null : overrides,
                Registration = registration,
                RegistrationType = type,
                Name = name,
                ExecutePlan = ContextExecutePlan,
                ResolvePlan = ContextResolvePlan,
                Type = registration is ContainerRegistration containerRegistration
                                     ? containerRegistration.Type : type
            };

            return ExecutePlan(ref context)!;
        }

        #endregion


        #region Child container management

        /// <inheritdoc />
        IUnityContainer IUnityContainer.CreateChildContainer()
        {
            var child = new UnityContainer(this);
            ChildContainerCreated?.Invoke(this, new ChildContainerCreatedEventArgs(child._context));
            return child;
        }

        /// <inheritdoc />
        IUnityContainer? IUnityContainer.Parent => _parent;

        #endregion
    }
}
