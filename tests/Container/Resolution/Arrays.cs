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
            Container.ResolveAll(typeof(Service));
            var child = Container.CreateChildContainer();
            var instance = child.ResolveAll(typeof(Service));

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service[]));
            Assert.AreEqual(0, ((Service[])instance).Length);
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
            Assert.IsInstanceOfType(results, typeof(int[]));
        }


        #region Test Data

        public class Service
        {
        }

        #endregion
    }
}
