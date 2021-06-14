using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Lifetime;

namespace Container
{
    public partial class Resolution
    {
        [TestMethod("Resolve Array"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArray()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);
        }


        [TestMethod("Resolve Array Twice"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayTwice()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance1 = Container.ResolveAll(typeof(Service));
            var instance2 = Container.ResolveAll(typeof(Service));

            Assert.AreNotSame(instance1, instance2);
        }


        [TestMethod("Resolve in Child"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayInChild()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            var child = Container.CreateChildContainer()
                                 .RegisterInstance("name", new Service())
                                 .CreateChildContainer();
            // Act
            var instance = child.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(4, ((Service[])instance).Length);
        }


        [TestMethod("Resolve 80 integers in Child"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayInChildAll100()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced, new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            var container = Container;

            // Arrange
            for (var j = 0; j < 4; j++)
            {
                var start = j * 20;
                var end = start + 20;

                for (var i = start; i < end; i++)
                {
                    container.RegisterInstance(i.ToString(), i);
                }
                container = container.CreateChildContainer();
            }
            container.RegisterInstance(200);

            // Act
            var instance = container.Resolve<int[]>();

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(int[]));
            Assert.AreEqual(80, instance.Length);
        }


        [TestMethod("Resolve 100 integers"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayAll100()
        {
            for (var i = 0; i < 100; i++)
            {
                Container.RegisterInstance(i.ToString(), i);
            }


            var results = Container.Resolve<int[]>();

            Assert.IsNotNull(results);
            Assert.AreEqual(100, results.Length);
            Assert.IsInstanceOfType(results, typeof(int[]));
        }


        [TestMethod("Resolve in Root than Child"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayRootChild()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced, new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);

            instance = Container.CreateChildContainer()
                                .ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);
        }


        [TestMethod("Resolve in Child than Root"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayChildRoot()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.CreateChildContainer()
                                    .ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);

            instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);
        }


        [TestMethod("Root, Change than Child"), TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayRootChangeChild()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced, new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);

            instance = Container.CreateChildContainer()
                                .RegisterType<Service>(string.Empty, new TransientLifetimeManager())
                                .ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(4, ((Service[])instance).Length);
        }
    }
}
