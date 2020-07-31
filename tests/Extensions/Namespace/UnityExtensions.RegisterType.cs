using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityExtensionsTests
    {
        [TestMethod]
        public void RegisterType()
        {
            // Act
            var descriptor = typeof(Foo).RegisterType();

            // Validate
            Assert.AreEqual(RegistrationCategory.Type, descriptor.Category);
            Assert.AreEqual(typeof(Foo), descriptor.Type);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultTypeLifetime.GetType());
        }

        [TestMethod]
        public void RegisterType_As()
        {
            // Act
            var descriptor = typeof(Foo).RegisterType(typeof(IFoo), typeof(IOtherFoo));

            // Validate
            Assert.AreEqual(RegistrationCategory.Type, descriptor.Category);
            Assert.AreEqual(typeof(Foo), descriptor.Type);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Factory);
            Assert.AreEqual(2, descriptor.RegisterAs.Length);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultTypeLifetime.GetType());
        }

        [TestMethod]
        public void RegisterType_WithInjectionMembers()
        {
            // Act
            var descriptor = typeof(Foo).RegisterType(typeof(IFoo), typeof(IOtherFoo))
                                        .WithInjectionMembers(new InjectionConstructor());
            // Validate
            Assert.AreEqual(RegistrationCategory.Type, descriptor.Category);
            Assert.AreEqual(typeof(Foo), descriptor.Type);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Factory);
            Assert.AreEqual(2, descriptor.RegisterAs.Length);
            Assert.AreEqual(1, descriptor.Manager.ToArray().Length);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultTypeLifetime.GetType());
        }

        [TestMethod]
        public void RegisterType_Lifetime()
        {
            // Act
            var descriptor = typeof(Foo).RegisterType(new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(RegistrationCategory.Type, descriptor.Category);
            Assert.AreEqual(typeof(Foo), descriptor.Type);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void RegisterType_Name()
        {
            // Act
            var descriptor = typeof(Foo).RegisterType(Name);

            // Validate
            Assert.AreEqual(RegistrationCategory.Type, descriptor.Category);
            Assert.AreEqual(typeof(Foo), descriptor.Type);
            Assert.AreEqual(Name, descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultTypeLifetime.GetType());
        }

        [TestMethod]
        public void RegisterType_Name_Lifetime()
        {
            // Act
            var descriptor = typeof(Foo).RegisterType(Name, new ContainerControlledLifetimeManager());

            // Validate
            Assert.AreEqual(RegistrationCategory.Type, descriptor.Category);
            Assert.AreEqual(typeof(Foo), descriptor.Type);
            Assert.AreEqual(Name, descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Factory);
            Assert.IsInstanceOfType(descriptor.Manager, typeof(ContainerControlledLifetimeManager));
        }
    }
}
