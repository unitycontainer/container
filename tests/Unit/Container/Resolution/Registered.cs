using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Lifetime;

namespace Container
{
    public partial class Resolution
    {
        [TestMethod("Resolve Transient"), TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Optimized()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.Resolve(typeof(Service), Optimized);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod("Resolve Hierarchical"), TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Balanced()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.Resolve(typeof(Service), Balanced);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod("Resolve Singleton"), TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Singleton()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.Resolve(typeof(Service), Singleton);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod("Resolve Singleton Twice"), TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Singleton_Twice()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance1 = Container.Resolve(typeof(Service), Singleton);
            var instance2 = Container.Resolve(typeof(Service), Singleton);

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(Service));
            
            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof(Service));

            Assert.AreSame(instance1, instance2);
        }


        [TestMethod("Resolve Singleton in Child"), TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Child()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.CreateChildContainer()
                                    .Resolve(typeof(Service), Singleton);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }
    }
}
