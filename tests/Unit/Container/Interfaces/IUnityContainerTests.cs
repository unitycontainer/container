using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;

namespace Container.Interfaces
{
    [TestClass]
    public class IUnityContainerTests
    {
        protected UnityContainer Container;

        [TestInitialize]
        public virtual void InitializeTest() => Container = new UnityContainer();

        [TestMethod]
        public void Registrations_IUnityContainer_First()
        {
            var registration = Container.Registrations.First();

            Assert.AreEqual(typeof(IUnityContainer), registration.RegisteredType);
        }

        [TestMethod]
        public void Registrations_IUnityContainerAsync_Present() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IUnityContainerAsync) == registration.RegisteredType));

        [TestMethod]
        public void Registrations_IServiceProvider_Present() =>
            Assert.IsTrue(Container.Registrations.Any(registration => typeof(IServiceProvider) == registration.RegisteredType));

        [TestMethod]
        public void Registrations_ToArray()
        {
            var registrations = Container.Registrations.ToArray();

            Assert.AreEqual(3, registrations.Length);
        }
    }
}
