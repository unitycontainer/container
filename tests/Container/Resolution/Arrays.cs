using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Lifetime;

namespace Container
{
    public partial class Resolution
    {
        [TestMethod, TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArray()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            Container.ResolveAll(typeof(Service));
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);
        }


        [TestMethod, TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayTwice()
        {
            // Arrange
            Container.RegisterType<Service>(Optimized, new TransientLifetimeManager())
                     .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                     .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());

            // Act
            Container.ResolveAll(typeof(Service));
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);
        }


        [TestMethod, TestProperty(RESOLVE, nameof(Array))]
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


        [TestMethod, TestProperty(RESOLVE, nameof(Array))]
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


        [TestMethod, TestProperty(RESOLVE, nameof(Array))]
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
    }
}
