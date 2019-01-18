using System;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Interface defining the behavior of the Unity dependency injection container.
    /// </summary>
    [CLSCompliant(true)]
    public interface IUnityContainer : IDisposable
    {
        /// <summary>
        /// Register a type mapping with the container, where the created instances will use
        /// the given <see cref="LifetimeManager"/>.
        /// </summary>
        /// <param name="typeFrom"><see cref="Type"/> that will be requested when resolving. Sometime it is called a ServiceType</param>
        /// <param name="typeTo"><see cref="Type"/> that will actually be returned. This type is also called ImplementationType.</param>
        /// <param name="name">Name to use for registration</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <param name="injectionMembers">Injection configuration objects. Can be null.</param>
        /// <remarks>Container will store registrations by <paramref name="typeFrom"/> type. Type <paramref name="typeTo"/> will not be registered 
        /// with the container and only used to build the requested instance.
        /// If type provided in <paramref name="typeTo"/> is already registered with container, registration creates mapping to the existing
        /// registration and instead will use registration for <paramref name="typeTo"/> type to create object.</remarks>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        /// <exception cref="InvalidRegistrationException"></exception>
        IUnityContainer RegisterType(Type typeFrom, Type typeTo, string name, ILifetimeManager lifetimeManager, params InjectionMember[] injectionMembers);


        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Instance registration is much like setting a type as a singleton, except that instead
        /// of the container creating the instance the first time it is requested, the user
        /// creates the instance ahead of type and adds that instance to the container.
        /// </para>
        /// </remarks>
        /// <param name="type"><see cref="Type"/> of instance to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="instance">Object to be registered</param>
        /// <param name="name">Name for registration</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> manager that controls how this instance will be managed by the container.
        /// Following are the only valid options: <see cref="ContainerControlledLifetimeManager"/>, <see cref="SingletonLifetimeManager"/>, <see cref="ExternallyControlledLifetimeManager"/>
        /// </param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        IUnityContainer RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager);


        /// <summary>
        /// Register <see cref="Type"/> factory with the container
        /// </summary>
        /// <param name="type"><see cref="Type"/> to register (may be an implemented interface instead of the actual type).</param>
        /// <param name="name">Name for registration</param>
        /// <param name="factory">Predefined <code>Func&lt;IUnityContainer, Type, string, object&gt;</code> to create types</param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance. This manager has to derive from <see cref="IFactoryLifetimeManager"/></param>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        IUnityContainer RegisterFactory(Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager);


        /// <summary>
        /// This is a fast way to check if type is registered with container
        /// </summary>
        /// <remarks>This method is quite fast. Although it is not free in terms of time spent, 
        /// it uses the same algorithm the container employs to obtain registrations and wastes very little time.</remarks>
        /// <param name="type"><see cref="Type"/> to look for</param>
        /// <param name="name">Name of the registration</param>
        /// <returns></returns>
        bool IsRegistered(Type type, string name);


        /// <summary>
        /// Lists all registrations available at this container.
        /// </summary>
        /// <remarks>
        /// This collection contains all registrations from this container as well
        /// as from all predecessor containers if this is a child container. Registrations
        /// from child containers override registrations with same type and name from
        /// parent containers.
        /// The sort order of returned registrations is not guaranteed in any way.
        /// </remarks>
        IEnumerable<IContainerRegistration> Registrations { get; }


        /// <summary>
        /// Resolve an instance of the requested type from the container.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of object to get typeFrom the container.</param>
        /// <param name="name">Name of the object to retrieve.</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        object Resolve(Type type, string name, params ResolverOverride[] overrides);


        /// <summary>
        /// Run an existing object through the container's build pipeline and perform injections on it.
        /// </summary>
        /// <remarks>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injections performed.
        /// </remarks>
        /// <param name="type"><see cref="Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to the object.</param>
        /// <param name="name">name to use when looking up the registration and other configurations.</param>
        /// <param name="overrides">Any overrides for the resolve calls.</param>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="type"/>).</returns>
        object BuildUp(Type type, object existing, string name, params ResolverOverride[] overrides);


        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <value>The parent container, or null if this container doesn't have one.</value>
        IUnityContainer Parent { get; }


        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different
        /// settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        IUnityContainer CreateChildContainer();
    }
}
