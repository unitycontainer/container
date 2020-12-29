using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;

namespace Container
{
    public partial class Resolution
    {
        [TestMethod, TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArray()
        {
            Container.ResolveAll(typeof(Service));
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(3, ((Service[])instance).Length);
        }


        [TestMethod, TestProperty(RESOLVE, nameof(Array))]
        public void ResolveArrayTwice()
        {
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
            IUnityContainer container = new UnityContainer();

            for (var i = 0; i < 100; i++)
            {
                container.RegisterInstance(i.ToString(), i);
            }


            var results = container.Resolve<int[]>();

            Assert.IsNotNull(results);
            Assert.AreEqual(100, results.Length);
            Assert.IsInstanceOfType(results, typeof(int[]));
        }
    }
}
