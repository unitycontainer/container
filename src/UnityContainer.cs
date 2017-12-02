// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Unity.Builder;
using Unity.Container.Registration;
using Unity.Events;
using Unity.Exceptions;
using Unity.Extension;
using Unity.Injection;
using Unity.Lifetime;
using Unity.ObjectBuilder;
using Unity.ObjectBuilder.Policies;
using Unity.Policy;
using Unity.Registration;
using Unity.Resolution;

namespace Unity
{
    /// <inheritdoc />
    /// <summary>
    /// A simple, extensible dependency injection container.
    /// </summary>
    public partial class UnityContainer : IUnityContainer
    {
        #region Public Constructor

        /// <summary>
        /// Create a default <see cref="UnityContainer"/>.
        /// </summary>
        public UnityContainer()
            : this(null)
        {
        }

        #endregion


        #region Type Registration

        /// <summary>
        /// RegisterType a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="typeFrom"><see cref="Type"/> that will be requested.</param>
        /// <param name="typeTo"><see cref="Type"/> that will actually be returned.</param>
        /// <param name="name">Name to use for registration, null if a default registration.</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RegisterType(Type typeFrom, Type typeTo, string name, LifetimeManager lifetimeManager, InjectionMember[] injectionMembers)
        {
            var to = typeTo ?? throw new ArgumentNullException(nameof(typeTo));

            if (string.IsNullOrEmpty(name))
            {
                name = null;
            }

            if (typeFrom != null && !typeFrom.GetTypeInfo().IsGenericType && !to.GetTypeInfo().IsGenericType)
            {
                if (!typeFrom.GetTypeInfo().IsAssignableFrom(to.GetTypeInfo()))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Constants.TypesAreNotAssignable,
                                                                                          typeFrom,
                                                                                          to), nameof(typeFrom));
                }
            }

            var buildKey = new NamedTypeBuildKey(typeFrom ?? to, name);
            _policies.Set<IBuildPlanPolicy>(new OverriddenBuildPlanMarkerPolicy(), buildKey);
            _policies.Clear<ILifetimePolicy>(buildKey);
            _policies.Clear<IBuildKeyMappingPolicy>(buildKey);

            _registeredNames.RegisterType(typeFrom ?? to, name);

            if (typeFrom != null && typeFrom != typeTo)
            {
                if (typeFrom.GetTypeInfo().IsGenericTypeDefinition && to.GetTypeInfo().IsGenericTypeDefinition)
                {
                    _policies.Set<IBuildKeyMappingPolicy>(new GenericTypeBuildKeyMappingPolicy(new NamedTypeBuildKey(to, name)),
                        new NamedTypeBuildKey(typeFrom, name));
                }
                else
                {
                    var policy = (null != injectionMembers && injectionMembers.Length > 0) || lifetimeManager is IRequireBuildUpPolicy
                        ? new BuildKeyMappingPolicy(new NamedTypeBuildKey(to, name))
                        : new ResolveMappingPolicy(new NamedTypeBuildKey(to, name));

                    _policies.Set<IBuildKeyMappingPolicy>(policy, new NamedTypeBuildKey(typeFrom, name));
                }
            }
            if (lifetimeManager != null)
            {
                SetLifetimeManager(typeFrom ?? to, name, lifetimeManager);
            }

            Registering?.Invoke(this, new RegisterEventArgs(typeFrom, to, name, lifetimeManager));

            if (null != injectionMembers && injectionMembers.Length > 0)
            {
                foreach (var member in injectionMembers)
                {
                    if (member is InjectionFactory || member is DelegateInjectionFactory)
                        throw new InvalidOperationException(Constants.CannotInjectFactory);

                    member.AddPolicies(typeFrom, to, name, _policies);
                }
            }

            return this;
        }

        #endregion


        #region Instance Registration

        /// <summary>
        /// RegisterType an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// </remarks>
        /// <param name="toType">Type of instance to register (may be an implemented interface instead of the full type).</param>
        /// <param name="instance">Object to returned.</param>
        /// <param name="name">Name for registration.</param>
        /// <param name="manager">
        /// <para>If true, the container will take over the lifetime of the instance,
        /// calling Dispose on it (if it's <see cref="IDisposable"/>) when the container is Disposed.</para>
        /// <para>
        ///  If false, container will not maintain a strong reference to <paramref name="instance"/>. User is responsible
        /// for disposing instance, and for keeping the instance typeFrom being garbage collected.</para></param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RegisterInstance(Type toType, string name, object instance, LifetimeManager manager)
        {
            if (null == instance) throw new ArgumentNullException(nameof(instance));
            if (null != toType) InstanceIsAssignable(toType, instance, nameof(instance));

            var type = toType ?? instance.GetType();
            var lifetime = manager ?? new ContainerControlledLifetimeManager();

            _registeredNames.RegisterType(type, name);

            lifetime.SetValue(instance);
            SetLifetimeManager(type, name, lifetime);

            if (lifetime is IBuildPlanPolicy buildPlanPolicy)
                _policies.Set(buildPlanPolicy, new NamedTypeBuildKey(type, name));

            RegisteringInstance?.Invoke(this, new RegisterInstanceEventArgs(type,
                                                              instance,
                                                              name,
                                                              lifetime));
            return this;
        }

        #endregion


        #region Getting objects

        /// <summary>
        /// GetOrDefault an instance of the requested type with the given name typeFrom the container.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of object to get typeFrom the container.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="resolverOverrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        public object Resolve(Type type, string name, params ResolverOverride[] resolverOverrides)
        {
            return BuildUp(type, null, name, resolverOverrides);
        }

        #endregion


        #region BuildUp existing object

        /// <summary>
        /// Run an existing object through the container and perform injection on it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is useful when you don'type control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injection performed.
        /// </para></remarks>
        /// <param name="typeToBuild"><see cref="Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to build up.</param>
        /// <param name="name">name to use when looking up the typemappings and other configurations.</param>
        /// <param name="resolverOverrides">Any overrides for the buildup.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="typeToBuild"/>).</returns>
        public object BuildUp(Type typeToBuild, object existing, string name, params ResolverOverride[] resolverOverrides)
        {
            var type = typeToBuild ?? throw new ArgumentNullException(nameof(typeToBuild));
            if (null != existing) InstanceIsAssignable(type, existing, nameof(existing));

            IBuilderContext context = null;

            try
            {
                context = new BuilderContext(this, _strategies.MakeStrategyChain(),
                                                   _lifetimeContainer,
                                                   _policies,
                                                   new NamedTypeBuildKey(type, name),
                                                   existing);
                context.AddResolverOverrides(resolverOverrides);

                if (type.GetTypeInfo().IsGenericTypeDefinition)
                {
                    throw new ArgumentException(
                        String.Format(CultureInfo.CurrentCulture,
                            Constants.CannotResolveOpenGenericType,
                            type.FullName), nameof(type));
                }

                return context.Strategies.ExecuteBuildUp(context);
            }
            catch (Exception ex)
            {
                throw new ResolutionFailedException(type, name, ex, context);
            }
        }

        #endregion


        #region Extension Management

        /// <summary>
        /// Add an extension object to the container.
        /// </summary>
        /// <param name="extension"><see cref="UnityContainerExtension"/> to add.</param>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer AddExtension(UnityContainerExtension extension)
        {
            _extensions.Add(extension ?? throw new ArgumentNullException(nameof(extension)));
            extension.InitializeExtension(new ContainerContext(this));

            return this;
        }

        /// <summary>
        /// GetOrDefault access to a configuration interface exposed by an extension.
        /// </summary>
        /// <remarks>Extensions can expose configuration interfaces as well as adding
        /// strategies and policies to the container. This method walks the list of
        /// added extensions and returns the first one that implements the requested type.
        /// </remarks>
        /// <param name="configurationInterface"><see cref="Type"/> of configuration interface required.</param>
        /// <returns>The requested extension's configuration interface, or null if not found.</returns>
        public object Configure(Type configurationInterface)
        {
            return _extensions.FirstOrDefault(ex => configurationInterface.GetTypeInfo()
                              .IsAssignableFrom(ex.GetType().GetTypeInfo()));
        }

        /// <summary>
        /// Remove all installed extensions typeFrom this container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method removes all extensions typeFrom the container, including the default ones
        /// that implement the out-of-the-box behavior. After this method, if you want to use
        /// the container again you will need to either read the default extensions or replace
        /// them with your own.
        /// </para>
        /// <para>
        /// The registered instances and singletons that have already been set up in this container
        /// do not get removed.
        /// </para>
        /// </remarks>
        /// <returns>The <see cref="UnityContainer"/> object that this method was called on (this in C#, Me in Visual Basic).</returns>
        public IUnityContainer RemoveAllExtensions()
        {
            var toRemove = new List<UnityContainerExtension>(_extensions);
            toRemove.Reverse();
            foreach (UnityContainerExtension extension in toRemove)
            {
                extension.Remove();
                (extension as IDisposable)?.Dispose();
            }

            _extensions.Clear();

            // Reset our policies, strategies, and registered names to reset to "zero"
            _strategies.Clear();
            _policies.ClearAll();
            _registeredNames.Clear();

            if (null == _parent)
                InitializeStrategies();

            return this;
        }

        #endregion


        #region Child container management

        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different
        /// settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        public IUnityContainer CreateChildContainer()
        {
            var child = new UnityContainer(this);
            var childContext = new ContainerContext(child);
            ChildContainerCreated?.Invoke(this, new ChildContainerCreatedEventArgs(childContext));
            return child;
        }

        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <value>The parent container, or null if this container doesn'type have one.</value>
        public IUnityContainer Parent => _parent;

        #endregion


        #region IDisposable Implementation

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// Disposing the container also disposes any child containers,
        /// and disposes any instances whose lifetimes are managed
        /// by the container.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose this container instance.
        /// </summary>
        /// <remarks>
        /// This class doesn'type have a finalizer, so <paramref name="disposing"/> will always be true.</remarks>
        /// <param name="disposing">True if being called typeFrom the IDisposable.Dispose
        /// method, false if being called typeFrom a finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_lifetimeContainer != null)
                {
                    _lifetimeContainer.Dispose();
                    _lifetimeContainer = null;

                    if (_parent != null && _parent._lifetimeContainer != null)
                    {
                        _parent._lifetimeContainer.Remove(this);
                    }
                }

                foreach (IDisposable disposable in Enumerable.OfType<IDisposable>(_extensions))
                    disposable.Dispose();

                _extensions.Clear();
            }
        }

        #endregion

        /// <summary>
        /// GetOrDefault a sequence of <see cref="ContainerRegistration"/> that describe the current state
        /// of the container.
        /// </summary>
        public IEnumerable<IContainerRegistration> Registrations
        {
            get
            {
                var allRegisteredNames = new Dictionary<Type, List<string>>();
                FillTypeRegistrationDictionary(allRegisteredNames);

                return
                    from type in allRegisteredNames.Keys
                    from name in allRegisteredNames[type]
                    select new ContainerRegistration(type, name, _policies);
            }
        }

        private void FillTypeRegistrationDictionary(IDictionary<Type, List<string>> typeRegistrations)
        {
            if (_parent != null)
            {
                _parent.FillTypeRegistrationDictionary(typeRegistrations);
            }

            foreach (Type t in _registeredNames.RegisteredTypes)
            {
                if (!typeRegistrations.ContainsKey(t))
                {
                    typeRegistrations[t] = new List<string>();
                }

                typeRegistrations[t] =
                    (typeRegistrations[t].Concat(_registeredNames.GetKeys(t))).Distinct().ToList();
            }
        }
    }
}
