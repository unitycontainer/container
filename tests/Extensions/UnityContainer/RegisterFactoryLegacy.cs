using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Generic Func<IUnityContainer, object>

        [TestMethod]
        public void RegisterFactory_Generic_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>((Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Generic_Manager_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

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
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>((Func<IUnityContainer, object>)null, (IFactoryLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterFactory_Generic_Name_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(name, factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, (Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Generic_Name_Manger_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

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
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, (Func<IUnityContainer, object>)null, (IFactoryLifetimeManager)manager));
        }

        #endregion

        #region  Generic Func<IUnityContainer, Type, string, object>

        [TestMethod]
        public void RegisterFactory_Generic_Full_Legacy()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>((Func<IUnityContainer, Type, string, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Generic_Manager_Full_Legacy()
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
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>((Func<IUnityContainer, Type, string, object>)null, (IFactoryLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterFactory_Generic_Name_Full_Legacy()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory<IUnityContainer>(name, factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory<IUnityContainer>(name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, (Func<IUnityContainer, Type, string, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Generic_Name_Manger_Full_Legacy()
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
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory<IUnityContainer>(name, (Func<IUnityContainer, Type, string, object>)null, (IFactoryLifetimeManager)manager));
        }

        #endregion


        #region Func<IUnityContainer, object>

        [TestMethod]
        public void RegisterFactory_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), (Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Manager_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

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
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), (Func<IUnityContainer, object>)null, (IFactoryLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterFactory_Name_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), name, factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, (Func<IUnityContainer, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Name_Manger_Legacy()
        {
            // Arrange
            Func<IUnityContainer, object> factory = (container) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), name, factory, (IFactoryLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), name, factory, (IFactoryLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, (Func<IUnityContainer, object>)null, (IFactoryLifetimeManager)manager));
        }

        #endregion

        #region Func<IUnityContainer, Type, string, object>

        [TestMethod]
        public void RegisterFactory_Full_Legacy()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), (Func<IUnityContainer, Type, string, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Manager_Full_Legacy()
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
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), (Func<IUnityContainer, Type, string, object>)null, (IFactoryLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterFactory_Name_Full_Legacy()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), name, factory);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNotNull(container.LifetimeManager);
            Assert.AreNotSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, name, factory));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, (Func<IUnityContainer, Type, string, object>)null));
        }

        [TestMethod]
        public void RegisterFactory_Name_Manger_Full_Legacy()
        {
            // Arrange
            Func<IUnityContainer, Type, string, object> factory = (container, type, name) => container;

            // Act
            container.RegisterFactory(typeof(IUnityContainer), name, factory, (IFactoryLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.IsNull(container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.AreSame(container, container.Data);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterFactory(typeof(IUnityContainer), name, factory, (IFactoryLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(null, name, factory, (IFactoryLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, factory, FakeManager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterFactory(typeof(IUnityContainer), name, (Func<IUnityContainer, Type, string, object>)null, (IFactoryLifetimeManager)manager));
        }

        #endregion
    }
}
