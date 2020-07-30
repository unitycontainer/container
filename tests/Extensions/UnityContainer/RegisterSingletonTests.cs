using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;
using Unity.Abstractions.Tests;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Generic Register Singleton

        [TestMethod]
        public void RegisterSingleton_Generic()
        {
            // Act
            container.RegisterSingleton<FakeUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterSingleton_Generic_FromTo()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<IUnityContainer, FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterSingleton_Name_Generic()
        {
            // Act
            container.RegisterSingleton<FakeUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreEqual(name, container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeUnityContainer>(name));
        }

        [TestMethod]
        public void RegisterSingleton_Name_Generic_FromTo()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<IUnityContainer, FakeUnityContainer>(name));
        }

        #endregion


        #region Register Singleton

        [TestMethod]
        public void RegisterSingleton()
        {
            // Act
            container.RegisterSingleton(typeof(FakeUnityContainer));

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterSingleton_Name()
        {
            // Act
            container.RegisterSingleton(typeof(FakeUnityContainer), name);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreEqual(name, container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeUnityContainer), name));
        }

        [TestMethod]
        public void RegisterSingleton_FromTo()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterSingleton_Name_FromTo()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer), name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.IsInstanceOfType(container.Descriptor.Manager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer), name));
        }

        #endregion
    }
}
