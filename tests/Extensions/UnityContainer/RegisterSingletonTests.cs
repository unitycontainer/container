using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
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
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterSingleton_Generic_FromTo()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<IUnityContainer, FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterSingleton_Name_Generic()
        {
            // Act
            container.RegisterSingleton<FakeUnityContainer>(name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeUnityContainer>(name));
        }

        [TestMethod]
        public void RegisterSingleton_Name_Generic_FromTo()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
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
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterSingleton_Name()
        {
            // Act
            container.RegisterSingleton(typeof(FakeUnityContainer), name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeUnityContainer), name));
        }

        [TestMethod]
        public void RegisterSingleton_FromTo()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterSingleton_Name_FromTo()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer), name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer), name));
        }

        #endregion
    }
}
