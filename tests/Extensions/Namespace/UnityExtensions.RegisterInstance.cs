using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityExtensionsTests
    {
        Foo Instance = new Foo();

        [TestMethod]
        public void RegisterInstance()
        {
            // Act
            var descriptor = Instance.RegisterInstance();

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, descriptor.Category);
            Assert.AreEqual(Instance, descriptor.Instance);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Type);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultInstanceLifetime.GetType());
        }

        [TestMethod]
        public void RegisterInstance_As()
        {
            // Act
            var descriptor = Instance.RegisterInstance(typeof(IFoo), typeof(IOtherFoo));

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, descriptor.Category);
            Assert.AreEqual(Instance, descriptor.Instance);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Type);
            Assert.IsNull(descriptor.Factory);
            Assert.AreEqual(2, descriptor.RegisterAs.Length);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultInstanceLifetime.GetType());
        }

        [TestMethod]
        public void RegisterInstance_WithInjectionMembers()
        {
            // Act
            var descriptor = Instance.RegisterInstance(typeof(IFoo), typeof(IOtherFoo))
                                        .WithInjectionMembers(new InjectionConstructor());
            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, descriptor.Category);
            Assert.AreEqual(Instance, descriptor.Instance);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Type);
            Assert.IsNull(descriptor.Factory);
            Assert.AreEqual(2, descriptor.RegisterAs.Length);
            Assert.AreEqual(1, descriptor.Manager.ToArray().Length);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultInstanceLifetime.GetType());
        }

        [TestMethod]
        public void RegisterInstance_Lifetime()
        {
            // Act
            var descriptor = Instance.RegisterInstance(new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, descriptor.Category);
            Assert.AreEqual(Instance, descriptor.Instance);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Type);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void RegisterInstance_Name()
        {
            // Act
            var descriptor = Instance.RegisterInstance(Name);

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, descriptor.Category);
            Assert.AreEqual(Instance, descriptor.Instance);
            Assert.AreEqual(Name, descriptor.Name);
            Assert.IsNull(descriptor.Type);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultInstanceLifetime.GetType());
        }

        [TestMethod]
        public void RegisterInstance_Name_Lifetime()
        {
            // Act
            var descriptor = Instance.RegisterInstance(Name, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(RegistrationCategory.Instance, descriptor.Category);
            Assert.AreEqual(Instance, descriptor.Instance);
            Assert.AreEqual(Name, descriptor.Name);
            Assert.IsNull(descriptor.Type);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, typeof(ContainerControlledLifetimeManager));
        }
    }
}
