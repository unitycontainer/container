using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Generic Register Instance

        [TestMethod]
        public void RegisterInstanceGeneric()
        {
            // Act
            container.RegisterInstance<IUnityContainer>(container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNull(container.InjectionMembers);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterInstance<IUnityContainer>(container));
        }

        [TestMethod]
        public void RegisterInstanceGenericWithManager()
        {
            // Act
            container.RegisterInstance<IUnityContainer>(container, (IInstanceLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNull(container.InjectionMembers);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterInstance<IUnityContainer>(container, (IInstanceLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterInstanceGenericWithName()
        {
            // Act
            container.RegisterInstance<IUnityContainer>(name, container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNull(container.InjectionMembers);                  // TODO: null vs array[]
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterInstance<IUnityContainer>(name, container));
        }

        [TestMethod]
        public void RegisterInstanceGenericWithNameAndManger()
        {
            // Act
            container.RegisterInstance<IUnityContainer>(name, container, (IInstanceLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNull(container.InjectionMembers);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterInstance<IUnityContainer>(name, container, (IInstanceLifetimeManager)manager));
        }

        #endregion


        #region Register Instance

        [TestMethod]
        public void RegisterInstance()
        {
            // Act
            container.RegisterInstance(typeof(IUnityContainer), container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNull(container.InjectionMembers);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterInstance(typeof(IUnityContainer), container));
        }

        [TestMethod]
        public void RegisterInstanceWithName()
        {
            // Act
            container.RegisterInstance(typeof(IUnityContainer), name, container);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNull(container.InjectionMembers);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterInstance(typeof(IUnityContainer), name, container));
        }

        [TestMethod]
        public void RegisterInstanceWithManager()
        {
            // Act
            container.RegisterInstance(typeof(IUnityContainer), container, (IInstanceLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNull(container.InjectionMembers);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterInstance(typeof(IUnityContainer), container, (IInstanceLifetimeManager)manager));
        }

        #endregion
    }
}
