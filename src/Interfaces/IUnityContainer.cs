using System;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Resolution;

namespace Unity
{
    /// <summary>
    /// <para>Interface defining the behavior of the Unity dependency injection container.</para>
    /// <para>This interface defines a compact API that allows registration of types, instances, and 
    /// factories, resolution of new objects or initialization of existing instances, scope manipulations,
    /// and etc.</para>
    /// </summary>
    [CLSCompliant(true)]
    public interface IUnityContainer : IDisposable
    {
        /// <summary>
        /// Name of this container
        /// </summary>
        string? Name { get; }

        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <remarks>
        /// If the instance of the container is a child container, this property will hold a reference to
        /// the container that created this instance.
        /// </remarks>
        /// <value>The parent container, or null if this container doesn't have one.</value>
        IUnityContainer? Parent { get; }


        IUnityContainer Register(params RegistrationDescriptor[] descriptors);


        IUnityContainer Register(in ReadOnlySpan<RegistrationDescriptor> span);


        /// <summary>
        /// Checks if <see cref="Type"/> is registered with container
        /// </summary>
        /// <param name="type"><see cref="Type"/> to look for</param>
        /// <param name="name">Name of the registration</param>
        /// <remarks>
        /// <para>This method checks if <see cref="Type"/> with the specified <paramref name="name"/> is registered with the container.</para>
        /// <para>
        /// When method verifies if <see cref="Type"/> is registered, it looks not only into the current container
        /// by all its predecessors as well. So if this <see cref="Type"/> not registered in the container but
        /// contained by one of its parents it will still return <c>True</c>.
        /// </para>
        /// <para>
        /// This method is quite fast. It uses the same algorithm the container employs to obtain registrations
        /// and has a very small overhead. It is an order of magnitude faster than querying <see cref="IUnityContainer.Registrations"/>
        /// collection.
        /// </para>
        /// </remarks>
        /// <returns><c>True</c> if <see cref="Type"/> is registered or <c>False</c> if no registration found</returns>
        bool IsRegistered(Type type, string? name);


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
        /// <seealso cref="ContainerRegistration"/>
        /// <value>Registered with the container types</value>
        IEnumerable<ContainerRegistration> Registrations { get; }


        /// <summary>
        /// Resolve an instance of the requested type from the container.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> of object to resolve</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="overrides">Overrides for dependencies</param>
        /// <remarks>
        /// <para>
        /// During resolution Unity checks if <see cref="Type"/> is registered and uses that registration to create an object. 
        /// If <see cref="Type"/> is not registered it uses reflection to get information about the <see cref="Type"/> and 
        /// creates a pipeline to instantiate and initialize the <see cref="Type"/> using reflected data.
        /// </para>
        /// <para>
        /// Resolver overrides passed to the method will only override dependencies configured for injection. For example, if 
        /// some members were marked with attributes to be injected or registered with associated <see cref="InjectionMember"/>
        /// objects, only these members will be available for overriding. Override values for members not configured for injection
        /// will be ignored.
        /// </para>
        /// <para>
        /// During resolution, Unity performs only a limited number of checks. If any errors occur, error information is very brief.
        /// To enable slower but more thorough and detailed validation and expanded error reporting add <c>Diagnostic</c>
        /// extension to the container. 
        /// </para>
        /// </remarks>
        /// <seealso cref="Unity.Override"/>
        /// <exception cref="ResolutionFailedException">Throws if any errors occur during resolution</exception>
        /// <returns>The retrieved object.</returns>
        object? Resolve(Type type, string? name, params ResolverOverride[] overrides);


        /// <summary>
        /// Run an existing object through the build pipeline and perform injections on it
        /// </summary>
        /// <param name="type"><see cref="Type"/> of object to perform injection on.</param>
        /// <param name="existing">Instance to the object.</param>
        /// <param name="name">name to use when looking up the registration and other configurations.</param>
        /// <param name="overrides">Any overrides for the resolve calls.</param>
        /// <remarks>
        /// <para>
        /// This method performs all the initializations and injections on an instance of an object you
        /// passed in <paramref name="existing"/>. 
        /// </para>
        /// <para>
        /// This method is useful when you don't control the construction of an
        /// instance (ASP.NET pages or objects created via XAML, for instance)
        /// but you still want properties and other injections performed.
        /// </para>
        /// </remarks>
        /// <seealso cref="Unity.Override"/>
        /// <exception cref="ResolutionFailedException">Throws if any errors occur during initialization</exception>
        /// <returns>The resulting object. By default, this will be <paramref name="existing"/>, but
        /// container extensions may add things like automatic proxy creation which would
        /// cause this to return a different object (but still type compatible with <paramref name="type"/>).</returns>
        object BuildUp(Type type, object existing, string? name, params ResolverOverride[] overrides);

        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <param name="name">Name of the child container</param>
        /// <param name="capacity">Preallocated capacity of child container</param>
        /// <remarks>
        /// Unity allows creating scopes with the help of child container. A child container shares the 
        /// parent's configuration but can be configured with different settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        IUnityContainer CreateChildContainer(string? name, int capacity);
    }
}
