using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Builder;
using Unity.Events;
using Unity.Exceptions;
using Unity.Injection;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Fields

        const string LifetimeManagerInUse = "The lifetime manager is already registered. Lifetime managers cannot be reused, please create a new one.";
        private Action<Type, Type> TypeValidator;

        #endregion


        #region Type Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterType(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {

            try
            {
                var mappedToType = typeTo;
                var registeredType = typeFrom ?? typeTo;

                // Validate input
                if (null == typeTo) throw new ArgumentNullException(nameof(typeTo));
                if (null == lifetimeManager) lifetimeManager = TransientLifetimeManager.Instance;
                if (lifetimeManager.InUse) throw new InvalidOperationException(LifetimeManagerInUse);

                // Validate are assignable
                TypeValidator?.Invoke(typeFrom, typeTo);

                // Create registration and add to appropriate storage
                var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
                var registration = new ContainerRegistration(_validators, typeTo, lifetimeManager, injectionMembers);

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
                if (lifetimeManager is IDisposable manager)
                    container.LifetimeContainer.Add(manager);

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
                                                                          lifetimeManager));
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
        IUnityContainer IUnityContainer.RegisterInstance(Type type, string name, object instance, LifetimeManager lifetimeManager)
        {
            var mappedToType = instance?.GetType();
            var typeFrom = type ?? mappedToType;

            try
            {
                // Validate input
                if (null == instance) throw new ArgumentNullException(nameof(instance));

                var lifetime = lifetimeManager ?? new ContainerControlledLifetimeManager();
                if (lifetime.InUse) throw new InvalidOperationException(LifetimeManagerInUse);
                lifetime.SetValue(instance, LifetimeContainer);

                // Create registration and add to appropriate storage
                var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
                var registration = new ContainerRegistration(null, mappedToType, lifetime);

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
                                                                       name, lifetimeManager));
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
                ExecutePlan = ExecutePlan,
                Type = registration is ContainerRegistration containerRegistration
                                     ? containerRegistration.Type : type,
            };

            return ExecuteDefaultPlan(ref context);
        }

        #endregion


        #region BuildUp existing object

        /// <inheritdoc />
        object IUnityContainer.BuildUp(Type type, object existing, string name, params ResolverOverride[] overrides)
        {
            // Verify arguments
            if (null == type) throw new ArgumentNullException(nameof(type));
            name = string.IsNullOrEmpty(name) ? null : name;
            if (null != existing) InstanceIsAssignable(type, existing, nameof(existing));

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
                ExecutePlan = ExecutePlan,
                Type = registration is ContainerRegistration containerRegistration
                                     ? containerRegistration.Type : type
            };

            return ExecuteDefaultPlan(ref context);
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
