using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Resolution;

namespace Extensions.Tests
{
    public partial class UnityExtensionsTests
    {
        ResolveDelegate<IResolveContext> Factory = (ref IResolveContext c) => new Foo();

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RegisterFactory()
        {
            // Act
            var descriptor = Factory.RegisterFactory();

            // Validate
            Assert.AreEqual(RegistrationCategory.Factory, descriptor.Category);
            Assert.AreSame(Factory, descriptor.Factory);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Type);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultFactoryLifetime.GetType());
        }

        [TestMethod]
        public void RegisterFactory_As()
        {
            // Act
            var descriptor = Factory.RegisterFactory(typeof(IFoo), typeof(IOtherFoo));

            // Validate
            Assert.AreEqual(RegistrationCategory.Factory, descriptor.Category);
            Assert.AreSame(Factory, descriptor.Factory);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Type);
            Assert.AreEqual(2, descriptor.RegisterAs.Length);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultFactoryLifetime.GetType());
        }

        [TestMethod]
        public void RegisterFactory_WithInjectionMembers()
        {
            // Act
            var descriptor = Factory.RegisterFactory(typeof(IFoo), typeof(IOtherFoo))
                                        .WithInjectionMembers(new InjectionConstructor());
            // Validate
            Assert.AreEqual(RegistrationCategory.Factory, descriptor.Category);
            Assert.AreSame(Factory, descriptor.Factory);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Type);
            Assert.AreEqual(2, descriptor.RegisterAs.Length);
            Assert.AreEqual(1, descriptor.Manager.ToArray().Length);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultFactoryLifetime.GetType());
        }

        [TestMethod]
        public void RegisterFactory_Lifetime()
        {
            // Act
            var descriptor = Factory.RegisterFactory(new ContainerControlledLifetimeManager(), typeof(IFoo));

            // Validate
            Assert.AreEqual(RegistrationCategory.Factory, descriptor.Category);
            Assert.AreSame(Factory, descriptor.Factory);
            Assert.IsNull(descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Type);
            Assert.IsInstanceOfType(descriptor.Manager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void RegisterFactory_Name()
        {
            // Act
            var descriptor = Factory.RegisterFactory(Name, typeof(IFoo));

            // Validate
            Assert.AreEqual(RegistrationCategory.Factory, descriptor.Category);
            Assert.AreSame(Factory, descriptor.Factory);
            Assert.AreEqual(Name, descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Type);
            Assert.IsInstanceOfType(descriptor.Manager, LifetimeManager.DefaultFactoryLifetime.GetType());
        }

        [TestMethod]
        public void RegisterFactory_Name_Lifetime()
        {
            // Act
            var descriptor = Factory.RegisterFactory(Name, new ContainerControlledLifetimeManager(), typeof(IFoo));

            // Validate
            Assert.AreEqual(RegistrationCategory.Factory, descriptor.Category);
            Assert.AreSame(Factory, descriptor.Factory);
            Assert.AreEqual(Name, descriptor.Name);
            Assert.IsNull(descriptor.Instance);
            Assert.IsNull(descriptor.Type);
            Assert.IsInstanceOfType(descriptor.Manager, typeof(ContainerControlledLifetimeManager));
        }
    }
}
