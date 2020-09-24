using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Lifetime;

namespace Container.Resolution
{
    [TestClass]
    public class Basics
    {
        const string Balanced  = "balanced";
        const string Singleton = "singleton";
        const string Optimized = "optimized";

        #region Scaffolding

        protected IUnityContainer Container;

        [TestInitialize]
        public void TestInitialize()
        {
            Container = new UnityContainer()
                .RegisterType<Service>(Optimized, new TransientLifetimeManager())
                .RegisterType<Service>(Balanced,  new HierarchicalLifetimeManager())
                .RegisterType<Service>(Singleton, new ContainerControlledLifetimeManager());
        }

        #endregion


        [TestMethod]
        public void Resolve_Registered_Optimized()
        {
            var instance = Container.Resolve(typeof(Service), Optimized);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod]
        public void Resolve_Registered_Balanced()
        {
            var instance = Container.Resolve(typeof(Service), Balanced);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod]
        public void Resolve_Registered_Singleton()
        {
            var instance = Container.Resolve(typeof(Service), Singleton);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }


        [TestMethod]
        public void Resolve_Registered_Singleton_Twice()
        {
            var instance1 = Container.Resolve(typeof(Service), Singleton);
            var instance2 = Container.Resolve(typeof(Service), Singleton);

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(Service));
            
            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof(Service));

            Assert.AreSame(instance1, instance2);
        }

        #region Test Data

        public class Service
        {
        }

        #endregion
    }
}
