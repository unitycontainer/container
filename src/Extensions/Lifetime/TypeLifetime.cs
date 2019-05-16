using Unity.Lifetime;

namespace Unity
{
    public static class TypeLifetime
    {
        /// <summary>
        /// Singleton lifetime creates globally unique singleton. Any Unity container 
        /// tree (parent and all the children) is guaranteed to have only one global 
        /// singleton for the registered type.
        /// </summary>
        /// <remarks>
        /// <para>Registering a type with singleton lifetime always places the registration 
        /// at the root of the container tree and makes it globally available for all 
        /// the children of that container. It does not matter if registration takes 
        /// places at the root of child container the destination is always the root node.</para>
        /// <para>Repeating the registration on any of the child nodes with singleton lifetime 
        /// will always override the root registration.</para>
        /// </remarks>
        /// <value>A new instance of a <see cref="SingletonLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager Singleton => new SingletonLifetimeManager();

        /// <summary>
        /// Unity returns the same instance each time the Resolve(...) method is called or when 
        /// the dependency mechanism injects the instance into other classes.
        /// </summary>
        /// <remarks>
        /// Per Container lifetime allows a registration of an existing or resolved object as 
        /// a scoped singleton in the container it was created or registered. In other words this 
        /// instance is unique within the container it war registered with. Child or parent 
        /// containers could have their own instances registered for the same contract.
        /// </remarks>
        /// <value>A new instance of a <see cref="ContainerControlledLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager ContainerControlled => new ContainerControlledLifetimeManager();

        /// <summary>
        /// Unity returns the same instance each time the Resolve(...) method is called or when 
        /// the dependency mechanism injects the instance into other classes.
        /// </summary>
        /// <remarks>
        /// Per Container lifetime allows a registration of an existing or resolved object as 
        /// a scoped singleton in the container it was created or registered. In other words this 
        /// instance is unique within the container it war registered with. Child or parent 
        /// containers could have their own instances registered for the same contract.
        /// </remarks>
        /// <value>A new instance of a <see cref="ContainerControlledLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager PerContainer => new ContainerControlledLifetimeManager();

        /// <summary>
        /// Unity returns a unique value for each child container.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Unity container allows creating hierarchies of child containers. This lifetime 
        /// creates local singleton for each level of the hierarchy. So, when you resolve a 
        /// type and this container does not have an instance of that type, the container will 
        /// create new instance. Next type the type is resolved the same instance will be returned.
        /// </para>
        /// <para>
        /// If a child container is created and requested to resolve the type, the child container 
        /// will create a new instance and store it for subsequent resolutions. Next time the 
        /// child container requested to resolve the type, it will return stored instance.
        /// </para>
        /// <para>If you have multiple children, each will resolve its own instance.</para>
        /// </remarks>
        /// <value>A new instance of a <see cref="HierarchicalLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager Hierarchical => new HierarchicalLifetimeManager();

        /// <summary>
        /// Unity returns a unique value for each scope.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The Unity container allows creating hierarchies of child containers. This lifetime 
        /// creates local singleton for each level of the hierarchy. So, when you resolve a 
        /// type and this container does not have an instance of that type, the container will 
        /// create new instance. Next type the type is resolved the same instance will be returned.
        /// </para>
        /// <para>
        /// If a child container is created and requested to resolve the type, the child container 
        /// will create a new instance and store it for subsequent resolutions. Next time the 
        /// child container requested to resolve the type, it will return stored instance.
        /// </para>
        /// <para>If you have multiple children, each will resolve its own instance.</para>
        /// </remarks>
        /// <value>A new instance of a <see cref="HierarchicalLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager Scoped => new HierarchicalLifetimeManager();

        /// <summary>
        /// This lifetime keeps a reference to an instance only for the duration of one resolution call
        /// </summary>
        /// <remarks>
        /// This type of lifetime is useful when you need to pass the same instance of the dependency 
        /// to a different nodes of the resolution graph.
        /// </remarks>
        /// <example>
        /// Consider this scenario:
        /// <code>
        /// class a {}
        /// 
        /// class b 
        /// {
        ///     b(a arg1)
        ///     {...}
        /// }
        /// 
        /// class c
        /// {
        ///     c(a arg1, b arg2)
        ///     {...}
        /// }
        /// </code>
        /// <para>
        /// When you resolve type `c`, it depends on type `b` and type `a`. Type `b`, in turn, 
        /// also depends on type `a`, and both types, `c` and `b`, require `a` to be the same instance.
        /// </para>
        /// <para>
        /// If type `a` is a singleton, the logic is easy. But if you require each instance of
        /// `c` to have a unique `a`, you could use per resolve lifetime. The instance of `a` 
        /// will act as a singleton only during that one resolution. Next call to resolve the 
        /// dependent type will create a new object.
        /// </para>
        /// <para>
        /// In the case of recursion, the singleton behavior is still applies and prevents circular dependency
        /// </para>
        /// </example>
        /// <value>A new instance of a <see cref="PerResolveLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager PerResolve => new PerResolveLifetimeManager();

        /// <summary>
        /// Per thread lifetime means a new instance of the registered <see cref="System.Type"/> 
        /// will be created once per each thread. In other words, if a Resolve{T}() method is
        /// called on a thread the first time, it will return a new object. Each subsequent 
        /// call to Resolve{T}(), or when the dependency mechanism injects instances of 
        /// the type into other classes on the same thread, the container will return the 
        /// same object.
        /// </summary>
        /// <value>A new instance of a <see cref="PerThreadLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager PerThread => new PerThreadLifetimeManager();

        /// <summary>
        /// This lifetime creates and returns a new instance of the requested type for each call 
        /// to the Resolve(...) method.
        /// </summary>
        /// <remarks>
        /// Transient lifetime is a default lifetime of the Unity container. As the name implies it 
        /// lasts very short period of time, actually, no time at all. In the Unity container terms, 
        /// having transient lifetime is the same as having no lifetime manager at all.
        /// </remarks>
        /// <value>An instance of a <see cref="TransientLifetimeManager"/> object.</value>
        public static ITypeLifetimeManager Transient => TransientLifetimeManager.Instance;

        /// <summary>
        /// This lifetime is similar to <see cref="TransientLifetimeManager"/> with exception 
        /// how the container holds references to created objects.
        /// </summary>
        /// <remarks>
        /// <para>
        /// On each call to the Resolve{T}() method a container will create a new objects.
        /// If the objects implements <see cref="System.IDisposable"/>, the container will hold a 
        /// reference to the interface and will dispose the object when the container goes out of scope.
        /// </para>
        /// <para>
        /// This lifetime is particularly useful in session based designs with child containers 
        /// associated with the session
        /// </para>
        /// </remarks>
        /// <value>A new instance of a <see cref="ContainerControlledTransientManager"/> object.</value>
        public static ITypeLifetimeManager PerContainerTransient => new ContainerControlledTransientManager();

        /// <summary>
        /// This lifetime keeps a weak reference to object it holds.
        /// </summary>
        /// <remarks>
        /// <para>When no object is associated with the manager container creates and returns a new object. 
        /// It gets and holds a weak reference to the created object. As long as the object still exists and 
        /// has not been garbage collected the container will return the object when requested.</para>
        /// <para>If the object went out of scope and has been garbage collected the container will 
        /// create and return a new instance.</para>
        /// <para>This lifetime manager does not dispose an object when container is disposed</para>
        /// </remarks>
        /// <value>A new instance of a <see cref="WeakReferenceLifetimeManager"/> lifetime manager.</value>
        public static ITypeLifetimeManager External => new ExternallyControlledLifetimeManager();
    }
}
