using Unity.Lifetime;

namespace Unity
{
    public static class FactoryLifetime
    {
        public static IFactoryLifetimeManager Singleton => new SingletonLifetimeManager();

        public static IFactoryLifetimeManager PerContainer => new ContainerControlledLifetimeManager();

        public static IFactoryLifetimeManager Hierarchical => new HierarchicalLifetimeManager();

        public static IFactoryLifetimeManager PerResolve => new PerResolveLifetimeManager();

        public static IFactoryLifetimeManager PerThread => new PerThreadLifetimeManager();

        public static IFactoryLifetimeManager Transient { get; } = new TransientLifetimeManager();

        public static IFactoryLifetimeManager PerContainerTransient => new ContainerControlledTransientManager();
    }
}
