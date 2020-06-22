using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Generic Register Factory

        [TestMethod]
        public void RegisterFactoryGeneric()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>((Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactoryGenericWithManager()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(factory, (IFactoryLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(factory, (IFactoryLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>((Func<IUnityContainer, Type, string, object>)null, (IFactoryLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterFactoryGenericWithName()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(name, factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, (Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactoryGenericWithNameAndManger()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(name, factory, (IFactoryLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(name, factory, (IFactoryLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, (Func<IUnityContainer, Type, string, object>)null, (IFactoryLifetimeManager)manager));
        }

        #endregion


        #region Register Factory

        [TestMethod]
        public void RegisterFactory()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), (Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactoryWithManager()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), factory, (IFactoryLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), factory, (IFactoryLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, factory, (IFactoryLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), (Func<IUnityContainer, Type, string, object>)null, (IFactoryLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterFactoryWithName()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), name, factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, (Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactoryWithNameAndManger()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), name, factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, (Func<IUnityContainer, Type, string, object>)null));
        }

        #endregion
    }
}
