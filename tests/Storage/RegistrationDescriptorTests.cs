using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Lifetime;
using Unity.Resolution;

namespace Storage.Tests
{
    [TestClass]
    public class RegistrationDescriptorTests
    {
        PolicyManager manager = new ContainerControlledLifetimeManager();

        [TestMethod]
        public void Baseline()
        {
            // Arrange
            var registration = new RegistrationDescriptor();

            // Validate
            Assert.AreEqual(RegistrationCategory.Uninitialized, registration.Category);
            Assert.IsNull(registration.Type);
            Assert.IsNull(registration.Instance);
            Assert.IsNull(registration.Factory);
            Assert.IsTrue(registration.ToString().StartsWith("Registration"));
        }

        [TestMethod]
        public void TypeRegistration()
        {
            // Arrange
            var registration = new RegistrationDescriptor(typeof(object), null, (ITypeLifetimeManager)manager, typeof(object));

            // Validate
            Assert.AreEqual(RegistrationCategory.Type, registration.Category);
            Assert.AreEqual(typeof(object), registration.Type);
            Assert.IsNull(registration.Instance);
            Assert.IsNull(registration.Factory);
        }

        [TestMethod]
        public void InstanceRegistration()
        {
            // Arrange
            var registration = new RegistrationDescriptor(this, null, (IInstanceLifetimeManager)manager, typeof(object));

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, registration.Category);
            Assert.IsNull(registration.Type);
            Assert.AreSame(this, registration.Instance);
            Assert.IsNull(registration.Factory);
        }

        [TestMethod]
        public void InstanceInvalid()
        {
            // Arrange
            var registration = new RegistrationDescriptor(this, null, (IInstanceLifetimeManager)manager);

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, registration.Category);
            Assert.IsNull(registration.Type);
            Assert.AreSame(this, registration.Instance);
            Assert.IsNull(registration.Factory);
        }

        [TestMethod]
        public void InstanceNullRegistration()
        {
            // Arrange
            var registration = new RegistrationDescriptor(null, null, (IInstanceLifetimeManager)manager, typeof(object));

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, registration.Category);
            Assert.IsNull(registration.Type);
            Assert.IsNull(registration.Instance);
            Assert.IsNull(registration.Factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void InstanceNullInvalid()
        {
            // Validate
            var registration = new RegistrationDescriptor(null, null, (IInstanceLifetimeManager)manager);
        }

        [TestMethod]
        public void FactoryRegistration()
        {
            ResolveDelegate<IResolveContext> factory = (ref IResolveContext c) => null;

            // Arrange
            var registration = new RegistrationDescriptor(factory, null, (IFactoryLifetimeManager)manager, typeof(object));

            // Validate
            Assert.AreEqual(RegistrationCategory.Factory, registration.Category);
            Assert.IsNull(registration.Type);
            Assert.IsNull(registration.Instance);
            Assert.IsNotNull(registration.Factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FactoryInvalid()
        {
            ResolveDelegate<IResolveContext> factory = (ref IResolveContext c) => null;

            // Arrange
            var registration = new RegistrationDescriptor(factory, null, (IFactoryLifetimeManager)manager);
        }
    }
}
