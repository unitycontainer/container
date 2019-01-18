using Unity.Lifetime;

namespace Unity
{
    public static class InstanceLifetime
    {
        public static IInstanceLifetimeManager External => new ExternallyControlledLifetimeManager();

        public static IInstanceLifetimeManager Singleton => new SingletonLifetimeManager();

        public static IInstanceLifetimeManager PerContainer => new ContainerControlledLifetimeManager();
    }
}
