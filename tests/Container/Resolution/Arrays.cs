using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container.Resolution
{
    [TestClass]
    public class Arrays
    {
        #region Scaffolding

        protected IUnityContainer Container;

        [TestInitialize]
        public void TestInitialize()
        {
            Container = new UnityContainer();
        }

        #endregion

        [TestMethod]
        public void Baseline()
        {
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(0, ((Service[])instance).Length);
        }


        [TestMethod]
        public void ResolveTwice()
        {
                           Container.ResolveAll(typeof(Service));
            var instance = Container.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(0, ((Service[])instance).Length);
        }

        [TestMethod]
        public void ResolveInChild()
        {
            // Arrange
            Container.RegisterInstance("name", new Service());
            var child = Container.CreateChildContainer()
                                 .CreateChildContainer();
            // Act
            var instance = child.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(1, ((Service[])instance).Length);
        }

        [TestMethod]
        public void ResolveInChildAll100()
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

        [TestMethod]
        public void ResolveAllof100()
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


        #region Test Data

        public class Service
        {
        }

        #endregion
    }
}
