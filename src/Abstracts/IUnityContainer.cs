using System;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Lifetime;
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
        /// Register a type or a type mapping with the container.
        /// </summary>
        /// <param name="registeredType">Registration <see cref="Type"/>. This <see cref="Type"/> will be 
        /// requested when resolving. Sometimes referred as <c>FromType</c> or <c>ServiceType</c></param>
        /// <param name="mappedToType">A <see cref="Type"/> that will actually be returned. Also referred
        /// as <c>ToType</c> or <c>ImplementationType</c>.</param>
        /// <param name="name">Name of the registration</param>
        /// <param name="lifetimeManager">A lifetime manager that controls how instances are created, managed, and disposed of. 
        /// If <paramref name="lifetimeManager"/> is <c>null</c>, container uses default <see cref="TypeLifetime.Transient"/> lifetime.</param>
        /// <param name="injectionMembers">Optional injection configuration objects</param>
        /// <remarks>
        /// <para>
        /// Container stores registrations by <paramref name="registeredType"/> <see cref="Type"/>. When resolving, it will look 
        /// for this <see cref="Type"/> to satisfy dependencies. Each registration is uniquely identified by <paramref name="registeredType"/> 
        /// and <paramref name="name"/>. Registering the same <see cref="Type"/> with different names will create distinct registrations 
        /// for each <see cref="Type"/> and <paramref name="name"/> combinations. Registration with no <paramref name="name"/> 
        /// (<c>name == null</c>) is called <c>default registration</c>. The container uses these as implicit defaults when required.
        /// </para>
        /// <para>
        /// Type <paramref name="mappedToType"/> will not be registered with the container. It will only be used to inform container how to 
        /// build the requested instance or where to redirect to satisfy the request. If the type provided in <paramref name="mappedToType"/> 
        /// is already registered with the container, the registration creates a mapping to the existing registration. It will redirect to that 
        /// registration when creating an object.
        /// </para>
        /// <para> 
        /// If <paramref name="injectionMembers"/> collection is not empty, the mapping will not redirect to other registrations. Instead, it will 
        /// always build the <see cref="Type"/> according to the rules set by provided <see cref="InjectionMember"/> objects.
        /// </para>
        /// <para>
        /// Registering a <see cref="Type"/> with the same <paramref name="name"/> second time will overwrite previous registration. When
        /// overwritten, registration will dispose of lifetime manager it was registered with and if that manager holds a reference to an instance
        /// of a disposable object (the object implements <see cref="IDisposable"/>), it will be disposed of as well.
        /// </para>
        /// <para>
        /// During registration, Unity performs only a limited number of checks. To enable slower but more thorough and detailed validation add 
        /// <c>Diagnostic</c> extension to the container. 
        /// </para>
        /// </remarks>
        /// <example>
        /// This example registers a default (no name) singleton service. The service will be created with a default constructor, field and property 
        /// <c>Resolved</c> and <c>Injected</c> are initialized with resolved and injected values respectively, and method <c>Init</c> is called on the 
        /// created object.
        /// <code>
        /// c.RegisterType(typeof(IService),                         // Type to register
        ///                typeof(Service),                          // Type to create
        ///                null,                                     // Default (no name)
        ///                TypeLifetime.Singleton,                   // Singleton lifetime
        ///                Invoke.Constructor(),                     // Use default ctor
        ///                Invoke.Method(nameof(Service.Init)),      // Call Init(...)
        ///                Resolve.Field(nameof(Service.Resolved))   // Resolve value for Resolved
        ///                Inject.Property(nameof(Service.Injected), // Inject Injected
        ///                                      "value"));          // with constant "value"
        /// </code>
        /// </example>
        /// <seealso cref="Unity.Inject"/>
        /// <seealso cref="Unity.Invoke"/>
        /// <seealso cref="Unity.Resolve"/>
        /// <seealso cref="Unity.TypeLifetime"/>
        /// <exception cref="InvalidOperationException">If error occur during registration container will throw an exception.</exception>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        IUnityContainer RegisterType(Type registeredType, Type mappedToType, string name, ITypeLifetimeManager lifetimeManager, params InjectionMember[] injectionMembers);


        /// <summary>
        /// Register an instance with the container.
        /// </summary>
        /// <param name="type">Registration <see cref="Type"/>. This parameter could be either <c>null</c> or any of the
        /// interface types the instance implements. If <paramref name="type"/> is <c>null</c> the instance will be registered by its <see cref="Type"/></param>
        /// <param name="instance">The Object to be registered</param>
        /// <param name="name">Registration name</param>
        /// <param name="lifetimeManager">An <see cref="InstanceLifetime"/> manager that controls the lifetime. If no manager is provided 
        /// (<c>lifetimeManager == null</c>) container uses <see cref="InstanceLifetime.PerContainer"/> lifetime by default</param>
        /// <remarks>
        /// <para>
        /// Instance registration makes objects created outside of the container to be available for dependency injection. Container registers 
        /// the instance as either <see cref="Type"/> provided in <paramref name="type"/>, or as  <see cref="Type"/> of the object itself (<c>instance.GetType()</c>).
        /// </para>
        /// <para>
        /// Instances created outside of container are treated as various types of singletons. There are three different lifetimes an instance supports:
        /// <list type="bullet">  
        ///     <item>  
        ///         <term><see cref="InstanceLifetime.External"/></term>  
        ///         <description>- Instance is managed elsewhere. The container holds just a weak reference to the instance. An author is responsible for 
        ///         making sure the instance is not going out of scope and garbage collected while still being used by the container.</description>  
        ///     </item>  
        ///     <item>  
        ///         <term><see cref="InstanceLifetime.Singleton"/></term>  
        ///         <description>- The instance is registered as a global singleton. This type of lifetime registers the instance with the root container
        ///         and makes it available to all descendants. It does not matter if an instance is registered with root container or any child containers, 
        ///         the registration is always stored at the root.</description>  
        ///     </item>  
        ///     <item>  
        ///         <term><see cref="InstanceLifetime.PerContainer"/></term>  
        ///         <description>- Instance is registered with a particular container and is available within the container and all its descendants.</description>  
        ///     </item>  
        /// </list>
        /// </para>
        /// </remarks>
        /// <example>
        /// This example registers a default (no name) service instance with externally controlled lifetime. 
        /// <code>
        /// c.RegisterInstance(typeof(IService),           // Type to register
        ///                    null,                       // Default (no name)
        ///                    instance,                   // Instance of Service
        ///                    InstanceLifetime.External); // Externally controlled
        /// </code>
        /// </example>
        /// <seealso cref="Unity.InstanceLifetime"/>
        /// <exception cref="InvalidOperationException">If types of registration and the instance are not assignable, method throws an exception</exception>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        IUnityContainer RegisterInstance(Type type, string name, object instance, IInstanceLifetimeManager lifetimeManager);


        /// <summary>
        /// Register <see cref="Type"/> factory with the container
        /// </summary>
        /// <param name="type">Registration <see cref="Type"/>. A <see cref="Type"/> the factory will create when requested</param>
        /// <param name="name">Registration name</param>
        /// <param name="factory">Predefined factory delegate</param>
        /// <param name="lifetimeManager">The <see cref="FactoryLifetime"/> that controls the lifetime of objects. 
        /// If <paramref name="lifetimeManager"/> is <c>null</c>, container uses default <see cref="TypeLifetime.Transient"/> lifetime</param>
        /// <remarks>
        /// <para>
        /// This method allows registration of factory function for specific <see cref="Type"/>. 
        /// </para>
        /// <para>
        /// This registration is very similar to <see cref="RegisterType(Type, Type, string, ITypeLifetimeManager, InjectionMember[])"/>
        /// except when registered <see cref="Type"/> is requested, instead of creating the <see cref="Type"/>, the container
        /// invokes registered factory delegate and returns the instance the delegate created.
        /// </para>
        /// </remarks>
        /// <example>
        /// This example registers a service factory with transient lifetime. 
        /// <code>
        /// c.RegisterInstance(typeof(IService),           // Type to register
        ///                    "Some Service",             // Registration name
        ///                    (c,t,n) => new Service(),   // Factory
        ///                    null);                      // Transient
        /// </code>
        /// </example>
        /// <seealso cref="Unity.FactoryLifetime"/>
        /// <exception cref="InvalidOperationException">If delegate is <c>null</c> method throws</exception>
        /// <returns>The <see cref="IUnityContainer"/> object that this method was called on.</returns>
        IUnityContainer RegisterFactory(Type type, string name, Func<IUnityContainer, Type, string, object> factory, IFactoryLifetimeManager lifetimeManager);


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
        bool IsRegistered(Type type, string name);


        /// <summary>
        /// Lists all registrations available at this container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This collection contains all registrations from this container. 
        /// </para>
        /// <para>
        /// If this is a child container <see cref="IUnityContainer.Registrations"/> will contain all registrations from this container 
        /// as well as registrations from all predecessor containers.  Registrations in child containers override registrations of the 
        /// same type and name from parent containers.
        /// </para>
        /// <para>
        /// The sort order of returned registrations is not guaranteed in any way.
        /// </para>
        /// </remarks>
        /// <seealso cref="IContainerRegistration"/>
        /// <value>Registered with the container types</value>
        IEnumerable<IContainerRegistration> Registrations { get; }


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
        object Resolve(Type type, string name, params ResolverOverride[] overrides);


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
        object BuildUp(Type type, object existing, string name, params ResolverOverride[] overrides);


        /// <summary>
        /// The parent of this container.
        /// </summary>
        /// <remarks>
        /// If the instance of the container is a child container, this property will hold a reference to
        /// the container that created this instance.
        /// </remarks>
        /// <value>The parent container, or null if this container doesn't have one.</value>
        IUnityContainer Parent { get; }


        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <remarks>
        /// Unity allows creating scopes with the help of child container. A child container shares the 
        /// parent's configuration but can be configured with different settings or lifetime.</remarks>
        /// <returns>The new child container.</returns>
        IUnityContainer CreateChildContainer();
    }
}
