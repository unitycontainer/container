using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Lifetime;

namespace Unity.Tests.v5
{
    [TestClass]
    public class DefaultLifetimeExtensionTests
    {
        [TestMethod]
        public void Register()
        {
            // Setup
            var container = new UnityContainer();

            // Act
            container.AddNewExtension<DefaultLifetime>();
            var config = container.Configure<DefaultLifetime>();

            // Validate
            Assert.IsNotNull(config);
        }

        [TestMethod]
        public void Defaults()
        {
            // Setup
            var container = new UnityContainer();

            // Act
            container.AddNewExtension<DefaultLifetime>();
            var config = container.Configure<DefaultLifetime>();

            // Validate
            Assert.IsNotNull(config);
            Assert.IsInstanceOfType(config.TypeDefaultLifetime,     typeof(TransientLifetimeManager));
            Assert.IsInstanceOfType(config.InstanceDefaultLifetime, typeof(ContainerControlledLifetimeManager));
            Assert.IsInstanceOfType(config.FactoryDefaultLifetime,  typeof(TransientLifetimeManager));
        }

        [TestMethod]
        public void GetSetValidation()
        {
            // Setup
            var manager = new TestLifetimeManager();
            var config = new UnityContainer().AddNewExtension<DefaultLifetime>()
                                             .Configure<DefaultLifetime>();

            // Act
            config.TypeDefaultLifetime = manager;
            config.InstanceDefaultLifetime = manager;
            config.FactoryDefaultLifetime = manager;

            // Validate
            Assert.IsNotNull(config);
            Assert.IsInstanceOfType(config.TypeDefaultLifetime,     typeof(TestLifetimeManager));
            Assert.IsInstanceOfType(config.InstanceDefaultLifetime, typeof(TestLifetimeManager));
            Assert.IsInstanceOfType(config.FactoryDefaultLifetime,  typeof(TestLifetimeManager));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TypeNull()
        {
            // Setup
            var config = new UnityContainer().AddNewExtension<DefaultLifetime>()
                                             .Configure<DefaultLifetime>();

            // Act
            config.TypeDefaultLifetime = null;

            // Validate
            Assert.Fail("Should throw above");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void InstanceNull()
        {
            // Setup
            var config = new UnityContainer().AddNewExtension<DefaultLifetime>()
                                             .Configure<DefaultLifetime>();

            // Act
            config.InstanceDefaultLifetime = null;

            // Validate
            Assert.Fail("Should throw above");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FactoryNull()
        {
            // Setup
            var config = new UnityContainer().AddNewExtension<DefaultLifetime>()
                                             .Configure<DefaultLifetime>();

            // Act
            config.FactoryDefaultLifetime = null;

            // Validate
            Assert.Fail("Should throw above");
        }
    }



    public class TestLifetimeManager : LifetimeManager, 
                                       ITypeLifetimeManager, 
                                       IInstanceLifetimeManager, 
                                       IFactoryLifetimeManager
    {
        protected override LifetimeManager OnCreateLifetimeManager() => 
            throw new System.NotImplementedException();
    }
}
