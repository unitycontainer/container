using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Registration
{
    public partial class Legacy
    {
        [TestMethod]
        public void SimpleObject()
        {
            // Arrange
            var instance = Guid.NewGuid().ToString();
            Container.RegisterInstance(instance);

            // Act/Validate
            Assert.AreEqual(Container.Resolve<string>(), instance);
        }

        [TestMethod]
        public void NamedObject()
        {
            // Arrange
            var instance = Guid.NewGuid().ToString();
            Container.RegisterInstance(instance, instance);

            // Act/Validate
            Assert.AreEqual(Container.Resolve<string>(instance), instance);
        }

        [TestMethod]
        public void InterfacedObject()
        {
            // Arrange
            var instance = new Service();
            Container.RegisterInstance<IService>(instance);

            // Act/Validate
            Assert.AreSame(instance, Container.Resolve<IService>());
            Assert.AreNotSame(instance, Container.Resolve<Service>());
        }

        [TestMethod]
        public void ExternallyControlledLifetimeManager()
        {
            // Arrange
            var instance = Guid.NewGuid().ToString();
            Container.RegisterInstance(instance.GetType(), null, instance, new ExternallyControlledLifetimeManager());

            // Act/Validate
            Assert.AreEqual(Container.Resolve<string>(), instance);
        }

        [TestMethod]
        public void RegisterWithParentAndChild()
        {
            //create unity container
            Container.RegisterInstance<string>(Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager());

            var child = Container.CreateChildContainer();
            child.RegisterInstance<string>(Guid.NewGuid().ToString(), new ContainerControlledLifetimeManager());

            // Act/Validate
            Assert.AreSame(Container.Resolve<string>(), Container.Resolve<string>());
            Assert.AreSame(child.Resolve<string>(), child.Resolve<string>());
            Assert.AreNotSame(Container.Resolve<string>(), child.Resolve<string>());
        }
    }
}
