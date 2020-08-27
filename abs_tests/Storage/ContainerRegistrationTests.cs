using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            ContainerRegistration registration = new ContainerRegistration();

            // Validate
            Assert.IsNull(registration.MappedToType);
        }

        [TestMethod]
        public void TypeManagerTest()
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
        public void ContractManagerTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager { Data = this };
            var contract = new Contract(typeof(object), Name);

            ContainerRegistration registration = new ContainerRegistration(in contract, manager);

            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.MappedToType);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }

        [TestMethod]
        public void TypeNameManagerTest()
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
        public void HashTypeNameManagerTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager { Data = this };

            ContainerRegistration registration = new ContainerRegistration(0, typeof(object), Name, manager);

            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreEqual(0, registration._contract.HashCode);
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
                Category = RegistrationCategory.Internal
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
                Category = RegistrationCategory.Internal
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
                Category = RegistrationCategory.Instance
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(GetType(), registration.MappedToType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.Factory);
            Assert.AreSame(this, registration.Instance);
        }


        [TestMethod]
        public void InstanceNullTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = null,
                Category = RegistrationCategory.Instance
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(typeof(object), registration.MappedToType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.Factory);
            Assert.IsNull(registration.Instance);
        }

        [TestMethod]
        public void InstanceNamedTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = this,
                Category = RegistrationCategory.Instance
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), Name, manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(GetType(), registration.MappedToType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.IsNull(registration.Factory);
            Assert.AreSame(this, registration.Instance);
        }

        [TestMethod]
        public void TypeTest()
        {
            // Arrange
            var manager = new TransientLifetimeManager
            {
                Data = typeof(string),
                Category = RegistrationCategory.Type
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
                Category = RegistrationCategory.Type
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
                Category = RegistrationCategory.Factory
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(typeof(object), registration.MappedToType);
            Assert.IsNull(registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
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
                Category = RegistrationCategory.Factory
            };

            // Act
            ContainerRegistration registration = new ContainerRegistration(typeof(object), Name, manager);

            // Validate
            Assert.AreEqual(typeof(object), registration.RegisteredType);
            Assert.AreEqual(typeof(object), registration.MappedToType);
            Assert.AreEqual(Name, registration.Name);
            Assert.AreSame(manager, registration.LifetimeManager);
            Assert.AreSame(factory, registration.Factory);
            Assert.IsNull(registration.Instance);
        }
    }
}
