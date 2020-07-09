using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace Storage.Tests
{
    [TestClass]
    public class ContainerRegistrationTests
    {
        private string Name = "0123456789";

        [TestMethod]
        public void Baseline()
        {
            // Arrange
            ContainerRegistration registration;

            // Validate
            Assert.IsNull(registration.RegisteredType);
            Assert.IsNull(registration.Name);
            Assert.IsNull(registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }

        [TestMethod]
        public void UnInitializedTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager { Data = this };

            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }

        [TestMethod]
        public void UnInitializedNamedTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager { Data = this };

            ContainerRegistration registration = new ContainerRegistration(typeof(object), Name, manager);

            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }


        [TestMethod]
        public void InternalTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager 
            {
                Data = this,
                RegistrationType = RegistrationType.Internal
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }

        [TestMethod]
        public void InternalNamedTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = this,
                RegistrationType = RegistrationType.Internal
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), Name, manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }



        [TestMethod]
        public void InstanceTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = this,
                RegistrationType = RegistrationType.Instance
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.AreSame(this, registration.Instance);
        }

        [TestMethod]
        public void InstanceNamedTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = this,
                RegistrationType = RegistrationType.Instance
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), Name, manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.AreSame(this, registration.Instance);
        }


        [TestMethod]
        public void TypelTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = typeof(string),
                RegistrationType = RegistrationType.Type
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.AreEqual(typeof(string), registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }

        [TestMethod]
        public void TypeNamedTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = typeof(string),
                RegistrationType = RegistrationType.Type
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), Name, manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.AreEqual(typeof(string), registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }


        [TestMethod]
        public void FactoryTest()
        {
            // Arrange
            ResolveDelegate<IResolveContext> factory = (ref IResolveContext c) => null;
            var manager = new TransientLifetimeManager
            {
                Data = factory,
                RegistrationType = RegistrationType.Factory
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.AreSame(factory, registration.Factory);
            Assert.IsNull(registration.Instance);
        }

        [TestMethod]
        public void FactoryNamedTest()
        {
            // Arrange
            ResolveDelegate<IResolveContext> factory = (ref IResolveContext c) => null;
            var manager = new TransientLifetimeManager
            {
                Data = factory,
                RegistrationType = RegistrationType.Factory
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), Name, manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.AreSame(factory, registration.Factory);
            Assert.IsNull(registration.Instance);
        }
    }
}
