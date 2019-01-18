using Unity.Lifetime;

namespace Unity
{
    public static class TypeLifetime
    {
        public static LifetimeManager External => new ExternallyControlledLifetimeManager();

        public static LifetimeManager Singleton => new SingletonLifetimeManager();

        public static LifetimeManager PerContainer => new ContainerControlledLifetimeManager();

        public static LifetimeManager Hierarchical => new HierarchicalLifetimeManager();

        public static LifetimeManager PerResolve => new PerResolveLifetimeManager();

        public static LifetimeManager PerThread => new PerThreadLifetimeManager();
        
        public static LifetimeManager Transient { get; } = new TransientLifetimeManager();

        public static LifetimeManager PerContainerTransient => new ContainerControlledTransientManager();
    }
}
