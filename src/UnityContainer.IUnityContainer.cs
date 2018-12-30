using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Events;
using Unity.Injection;
using Unity.Registration;
using Unity.Resolution;
using Unity.Storage;

namespace Unity
{
    public partial class UnityContainer : IUnityContainer
    {
        #region Type Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterType(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            // Validate input
            if (string.Empty == name) name = null;
            if (null == typeTo) throw new ArgumentNullException(nameof(typeTo));
            if (null == lifetimeManager) lifetimeManager = TransientLifetimeManager.Instance;
            if (lifetimeManager.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);
#if NETSTANDARD1_0 || NETCOREAPP1_0
            if (typeFrom != null && !typeFrom.GetTypeInfo().IsGenericType && !typeTo.GetTypeInfo().IsGenericType && 
                                    !typeFrom.GetTypeInfo().IsAssignableFrom(typeTo.GetTypeInfo()))
#else
            if (typeFrom != null && !typeFrom.IsGenericType && !typeTo.IsGenericType &&
                                    !typeFrom.IsAssignableFrom(typeTo))
#endif
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                    Constants.TypesAreNotAssignable, typeFrom, typeTo), nameof(typeFrom));
            }

            // Create registration and add to appropriate storage
            var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
            var registration = new ContainerRegistration(typeTo, lifetimeManager, injectionMembers);

            var registeredType = typeFrom ?? typeTo;
            var mappedToType = typeTo;

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
                var context = new RegistrationContext(this, registeredType, name, registration);
                foreach (var member in injectionMembers)
                {
                    member.AddPolicies<BuilderContext, RegistrationContext>(
                        registeredType, mappedToType,
                        name, ref context);
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
            return this;
        }

        #endregion


        #region Instance Registration

        /// <inheritdoc />
        IUnityContainer IUnityContainer.RegisterInstance(Type type, string name, object instance, LifetimeManager lifetimeManager)
        {
            // Validate input
            if (string.Empty == name) name = null;
            if (null == instance) throw new ArgumentNullException(nameof(instance));


            var mappedToType = instance.GetType();
            var typeFrom = type ?? mappedToType;
            var lifetime = lifetimeManager ?? new ContainerControlledLifetimeManager();
            if (lifetime.InUse) throw new InvalidOperationException(Constants.LifetimeManagerInUse);
            lifetime.SetValue(instance, LifetimeContainer);

            // Create registration and add to appropriate storage
            var container = lifetimeManager is SingletonLifetimeManager ? _root : this;
            var registration = new ContainerRegistration(mappedToType, lifetime);

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
            return this;
        }

        #endregion


        #region Registrations

        /// <inheritdoc />
        bool IUnityContainer.IsRegistered(Type type, string name) =>
            ReferenceEquals(string.Empty, name) ? _isTypeExplicitlyRegistered(type)
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
                list = new PolicyList(),
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
                list = new PolicyList(),
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
