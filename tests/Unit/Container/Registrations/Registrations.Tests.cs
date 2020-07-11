using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;

namespace Container.Registrations
{
    [TestClass]
    public class RegistrationsTests
    {
        protected IUnityContainer Container;

        [TestInitialize]
        public virtual void InitializeTest() => Container = new UnityContainer();

        [TestMethod]
        public void IUnityContainerIsFirst()
        {
            // Act 
            var registration = Container.Registrations.First();

            Assert.AreEqual(typeof(IUnityContainer), registration.RegisteredType);
            Assert.AreSame(Container, registration.LifetimeManager.Data);
        }

        [TestMethod]
        public void IUnityContainerAsyncIsPresent()
        {
            // Act 
            var registration = Container.Registrations
                                        .Take(2)
                                        .Last();

            Assert.AreEqual(typeof(IUnityContainerAsync), registration.RegisteredType);
            Assert.AreSame(Container, registration.LifetimeManager.Data);
        }

        [TestMethod]
        public void IServiceProviderIaPresent()
        {
            // Act 
            var registration = Container.Registrations
                                        .Take(3)
                                        .Last();

            Assert.AreEqual(typeof(IServiceProvider), registration.RegisteredType);
            Assert.AreSame(Container, registration.LifetimeManager.Data);
        }

        [TestMethod]
        public void Registrations_ToArray()
        {
            var registrations = Container.Registrations.ToArray();

            Assert.AreEqual(3, registrations.Length);
        }
    }
}
