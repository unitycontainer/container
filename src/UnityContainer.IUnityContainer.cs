using System;
using System.Collections.Generic;
using System.Linq;
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
        private Action<Type, Type> TypeValidator;

        #endregion


        #region Type Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterType(Type typeFrom, Type typeTo, string name, ITypeLifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            try
            {
                var mappedToType = typeTo;
                var registeredType = typeFrom ?? typeTo;

                // Validate input
                if (null == typeTo) throw new ArgumentNullException(nameof(typeTo));
                if (null == lifetimeManager) lifetimeManager = TransientLifetimeManager.Instance;
                if (((LifetimeManager)lifetimeManager).InUse) throw new InvalidOperationException(LifetimeManagerInUse);

                // Validate if they are assignable
                TypeValidator?.Invoke(typeFrom, typeTo);

                // Create registration and add to appropriate storage
                var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
                var registration = new ContainerRegistration(_validators, typeTo, (LifetimeManager)lifetimeManager, injectionMembers);

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
                if (lifetimeManager is IDisposable disposableManager)
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
                                                              registeredType, registration, injectionMembers))
                                                          .ToArray();
                // Raise event
                container.Registering?.Invoke(this, new RegisterEventArgs(registeredType,
                                                                          mappedToType,
                                                                          name,
                                                                          ((LifetimeManager)lifetimeManager)));
            }
            catch (Exception ex)
            {
                var parts = new List<string>();
                var generics = null == typeFrom ? typeTo?.Name : $"{typeFrom?.Name},{typeTo?.Name}";
                if (null != name) parts.Add($" '{name}'");
                if (null != lifetimeManager && !(lifetimeManager is TransientLifetimeManager)) parts.Add(lifetimeManager.ToString());
                if (null != injectionMembers && 0 != injectionMembers.Length)
                    parts.Add(string.Join(" ,", injectionMembers.Select(m => m.ToString())));

                var message = $"Error in  RegisterType<{generics}>({string.Join(", ", parts)})";
                throw new InvalidOperationException(message, ex);
            }

            return this;
        }

        #endregion


        #region Instance Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            var mappedToType = instance?.GetType();
            var typeFrom = type ?? mappedToType;

            try
            {
                // Validate input
                if (null == instance) throw new ArgumentNullException(nameof(instance));

                if (null == lifetimeManager) lifetimeManager = new ContainerControlledLifetimeManager();
                if (((LifetimeManager)lifetimeManager).InUse) throw new InvalidOperationException(LifetimeManagerInUse);
                ((LifetimeManager)lifetimeManager).SetValue(instance, LifetimeContainer);

                // Create registration and add to appropriate storage
                var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
                var registration = new ContainerRegistration(null, mappedToType, ((LifetimeManager)lifetimeManager));

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
                if (lifetimeManager is IDisposable manager)
                    container.LifetimeContainer.Add(manager);

                // Check what strategies to run
                registration.BuildChain = _strategiesChain.ToArray()
                                                     .Where(strategy => strategy.RequiredToResolveInstance(this, registration))
                                                     .ToArray();
                // Raise event
                container.RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(typeFrom, instance,
                                                                       name, ((LifetimeManager)lifetimeManager)));
            }
            catch (Exception ex)
            {
                var parts = new List<string>();

                if (null != name) parts.Add($" '{name}'");
                if (null != lifetimeManager && !(lifetimeManager is TransientLifetimeManager)) parts.Add(lifetimeManager.ToString());

                var message = $"Error in  RegisterInstance<{typeFrom?.Name}>({string.Join(", ", parts)})";
                throw new InvalidOperationException(message, ex);
            }

            return this;
        }

        #endregion


        #region Factory Registration

        /// <inheritdoc />
        public IUnityContainer RegisterFactory(Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager)
        {
            // Validate input
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            if (null == lifetimeManager) lifetimeManager = TransientLifetimeManager.Instance;
            if (((LifetimeManager)lifetimeManager).InUse) throw new InvalidOperationException(LifetimeManagerInUse);

            // Create registration and add to appropriate storage
            var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
#pragma warning disable CS0618 // TODO: InjectionFactory
            var injectionFactory = new InjectionFactory(factory);
#pragma warning restore CS0618
            var injectionMembers = new InjectionMember[] { injectionFactory };
            var registration = new ContainerRegistration(_validators, type, ((LifetimeManager)lifetimeManager), injectionMembers);

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
            if (lifetimeManager is IDisposable manager)
                container.LifetimeContainer.Add(manager);

            // Add Injection Members
            injectionFactory.AddPolicies<BuilderContext, ContainerRegistration>(
                type, type, name, ref registration);

            // Check what strategies to run
            registration.BuildChain = _strategiesChain.ToArray()
                                                      .Where(strategy => strategy.RequiredToBuildType(this,
                                                          type, registration, injectionMembers))
                                                      .ToArray();
            // Raise event
            container.Registering?.Invoke(this, new RegisterEventArgs(type, type, name,
                                                                      ((LifetimeManager)lifetimeManager)));
            return this;
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        bool IUnityContainer.IsRegistered(Type type, string name) => ReferenceEquals(All, name) ? IsTypeExplicitlyRegistered(type)
                                                                                                : _isExplicitlyRegistered(type, name);
        /// <inheritdoc />
        IEnumerable<IContainerRegistration> IUnityContainer.Registrations => GetRegistrations(this);

        #endregion


        #region Getting objects

        /// <inheritdoc />
        object IUnityContainer.Resolve(Type type, string name, params ResolverOverride[] overrides)
        {
            // Verify arguments
            if (null == type) throw new ArgumentNullException(nameof(type));
            name = string.IsNullOrEmpty(name) ? null : name;

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
                Type = registration is ContainerRegistration containerRegistration
                                     ? containerRegistration.Type : type,
            };

            return ExecutePlan(ref context);
        }

        #endregion


        #region BuildUp existing object

        /// <inheritdoc />
        object IUnityContainer.BuildUp(Type type, object existing, string name, params ResolverOverride[] overrides)
        {
            // Verify arguments
            if (null == type) throw new ArgumentNullException(nameof(type));

            // Validate if they are assignable
            if (null != existing && null != TypeValidator) TypeValidator(type, existing.GetType());

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
                Type = registration is ContainerRegistration containerRegistration
                                     ? containerRegistration.Type : type
            };

            return ExecutePlan(ref context);
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
        IUnityContainer IUnityContainer.Parent => _parent;

        #endregion
    }
}
