using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// Interface defining the behavior of the Unity dependency injection container.
    /// </summary>
    [CLSCompliant(true)]
    public interface IUnityContainerAsync : IDisposable
    {
        /// <summary>
        /// Registers implementation type <param name="injectionMembers"></param> via provided collection of it's interfaces.
        /// </summary>
        /// <remarks>
        /// This method allows creation of single registration for multiple interfaces the object of type <param name="type"></param>
        /// might be exposing. Registrations created with this method are self contained and will never 'Map' to other registrations.
        /// In other words this registration will always create build plan and resolve new objects through it.
        /// </remarks>
        /// <param name="interfaces">Collection of interfaces that <paramref name="type"/> exposes to container</param>
        /// <param name="type"><see cref="Type"/> that will be used to instantiate object.</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="lifetimeManager">Lifetime manager that will be responsible for managing created object's lifetime.</param>
        /// <param name="injectionMembers">Injection configuration objects.</param>
        /// <returns>Returns <see cref="Task"/> indicating when registration is done</returns>
        Task RegisterType(IEnumerable<Type> interfaces, Type type, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers);


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
        /// <param name="interfaces">Collection of interfaces that <paramref name="instance"/> exposes to container</param>
        /// <param name="instance">Object to be registered</param>
        /// <param name="name">Name for registration</param>
        /// <param name="lifetimeManager">
        /// <see cref="LifetimeManager"/> manager that controls how this instance will be managed by the container.
        /// Following are the only valid options: <see cref="ContainerControlledLifetimeManager"/>, <see cref="SingletonLifetimeManager"/>, <see cref="ExternallyControlledLifetimeManager"/>
        /// </param>
        /// <returns>Returns <see cref="Task"/> indicating when registration is done</returns>
        Task RegisterInstance(IEnumerable<Type> interfaces, string name, object instance, IInstanceLifetimeManager lifetimeManager);


        /// <summary>
        /// Register <see cref="Type"/> factory with the container
        /// </summary>
        /// <param name="interfaces">Collection of interfaces that <paramref name="factory"/> exposes to container</param>
        /// <param name="name">Name for registration</param>
        /// <param name="factory"></param>
        /// <param name="lifetimeManager">The <see cref="LifetimeManager"/> that controls the lifetime
        /// of the returned instance.</param>
        /// <returns>Returns <see cref="Task"/> indicating when registration is done</returns>
        Task RegisterFactory(IEnumerable<Type> interfaces, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager);


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
        ValueTask<object> ResolveAsync(Type type, string name, params ResolverOverride[] overrides);


        /// <summary>
        /// Resolve an instance of the requested type from the container.
        /// </summary>
        /// <param name="type"><see cref="Type"/> of object to get typeFrom the container.</param>
        /// <param name="regex">Pattern to match names to. Only these with successful 
        /// <see cref="Regex.IsMatch(string name)"/> will be resolved</param>
        /// <param name="overrides">Any overrides for the resolve call.</param>
        /// <returns>The retrieved object.</returns>
        ValueTask<IEnumerable<object>> Resolve(Type type, Regex regex, params ResolverOverride[] overrides);


        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <value>The parent container, or null if this container doesn't have one.</value>
        IUnityContainerAsync Parent { get; }


        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different
        /// settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        IUnityContainerAsync CreateChildContainer();
    }
}
