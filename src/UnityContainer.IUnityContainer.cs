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
        private Action<Type, Type> TypeValidator;

        #endregion


        #region Type Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterType(Type typeFrom, Type typeTo, string name, ITypeLifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            var mappedToType = typeTo;
            var registeredType = typeFrom ?? typeTo;
            if (null == registeredType) throw new ArgumentNullException(nameof(typeTo));

            // Validate if they are assignable
            TypeValidator?.Invoke(typeFrom, typeTo);

            try
            {
                LifetimeManager manager = (null != lifetimeManager) 
                                        ? (LifetimeManager)lifetimeManager 
                                        : TypeLifetimeManager.CreateLifetimePolicy();
                if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);

                // Create registration and add to appropriate storage
                var container = manager is SingletonLifetimeManager ? _root : this;
                var registration = new ContainerRegistration(_validators, typeTo, manager, injectionMembers);
                if (manager is ContainerControlledLifetimeManager lifeteime) lifeteime.Scope = container;

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
                                                              registeredType, registration, injectionMembers))
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
        IUnityContainer IUnityContainer.RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager)
        {
            var mappedToType = instance?.GetType();
            var typeFrom = type ?? mappedToType;
            LifetimeManager manager = (null != lifetimeManager)
                                    ? (LifetimeManager)lifetimeManager
                                    : InstanceLifetimeManager.CreateLifetimePolicy();
            try
            {
                // Validate input
                if (null == typeFrom) throw new InvalidOperationException($"At least one of Type arguments '{nameof(type)}' or '{nameof(instance)}' must be not 'null'");

                if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);
                manager.SetValue(instance, LifetimeContainer);

                // Create registration and add to appropriate storage
                var container = manager is SingletonLifetimeManager ? _root : this;
                var registration = new ContainerRegistration(null, mappedToType, manager);
                if (manager is ContainerControlledLifetimeManager lifeteime) lifeteime.Scope = container;

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
                container.RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(typeFrom, instance,
                                                                                          name, manager));
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
            LifetimeManager manager = (null != lifetimeManager)
                                    ? (LifetimeManager)lifetimeManager
                                    : FactoryLifetimeManager.CreateLifetimePolicy();
            // Validate input
            if (null == type) throw new ArgumentNullException(nameof(type));
            if (null == factory) throw new ArgumentNullException(nameof(factory));
            if (manager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);

            // Create registration and add to appropriate storage
            var container = manager is SingletonLifetimeManager ? _root : this;
#pragma warning disable CS0618 // TODO: InjectionFactory
            var injectionFactory = new InjectionFactory(factory);
#pragma warning restore CS0618
            var injectionMembers = new InjectionMember[] { injectionFactory };
            var registration = new ContainerRegistration(_validators, type, manager, injectionMembers);
            if (manager is ContainerControlledLifetimeManager lifeteime) lifeteime.Scope = container;

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

            // Add Injection Members
            injectionFactory.AddPolicies<BuilderContext, ContainerRegistration>(
                type, type, name, ref registration);

            // Check what strategies to run
            registration.BuildChain = _strategiesChain.ToArray()
                                                      .Where(strategy => strategy.RequiredToBuildType(this,
                                                          type, registration, injectionMembers))
                                                      .ToArray();
            // Raise event
            container.Registering?.Invoke(this, new RegisterEventArgs(type, type, name, manager));
            return this;
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        bool IUnityContainer.IsRegistered(Type type, string name) => ReferenceEquals(All, name) ? IsTypeExplicitlyRegistered(type)
                                                                                                : _isExplicitlyRegistered(type, name);
        #endregion


        #region Getting objects

        /// <inheritdoc />
        object IUnityContainer.Resolve(Type type, string name, params ResolverOverride[] overrides)
        {
            // Verify arguments
            if (null == type) throw new ArgumentNullException(nameof(type));

            var registration = (InternalRegistration)GetRegistration(type, name);
            var container = registration.Get(typeof(LifetimeManager)) is ContainerControlledLifetimeManager manager
                          ? (UnityContainer)manager.Scope 
                          : this;

            var context = new BuilderContext
            {
                List = new PolicyList(),
                Lifetime = container.LifetimeContainer,
                Overrides = null != overrides && 0 == overrides.Length ? null : overrides,
                Registration = registration,
                RegistrationType = type,
                Name = name,
                ExecutePlan = ContextExecutePlan,
                ResolvePlan = ContextResolvePlan,
                Type = registration is ContainerRegistration containerRegistration ? containerRegistration.Type : type,
            };

            return container.ExecutePlan(ref context);
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
            var container = registration.Get(typeof(LifetimeManager)) is ContainerControlledLifetimeManager manager
                          ? (UnityContainer)manager.Scope
                          : this;

            var context = new BuilderContext
            {
                List = new PolicyList(),
                Lifetime = container.LifetimeContainer,
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
