using Unity.Lifetime;

namespace Unity
{
    public static class InstanceLifetime
    {
        public static IInstanceLifetimeManager External => new ExternallyControlledLifetimeManager();

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
        public static IInstanceLifetimeManager Singleton => new SingletonLifetimeManager();

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
        public static IInstanceLifetimeManager PerContainer => new ContainerControlledLifetimeManager();
    }
}
