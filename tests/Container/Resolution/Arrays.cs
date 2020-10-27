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

        #region Test Data

        public class Service
        {
        }

        #endregion
    }
}
