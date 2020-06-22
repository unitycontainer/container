using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Abstractions.Tests;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Generic Register Type

        [TestMethod]
        public void RegisterTypeGeneric()
        {
            // Act
            container.RegisterType<FakeUnityContainer>();

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterTypeToGeneric()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterTypeGenericWithManager()
        {
            // Act
            container.RegisterType<FakeUnityContainer>((ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);           // TODO: null vs array[]
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>((ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeToGenericWithManager()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>((ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>((ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeGenericWithName()
        {
            // Act
            container.RegisterType<FakeUnityContainer>(name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>(name));
        }

        [TestMethod]
        public void RegisterTypeToGenericWithName()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>(name));
        }

        [TestMethod]
        public void RegisterTypeGenericWithNameAndManager()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType<FakeUnityContainer>(name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>(name, (ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeToGenericWithNameAndManager()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>(name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>(name, (ITypeLifetimeManager)manager));
        }

        #endregion


        #region Generic Register Singleton

        [TestMethod]
        public void RegisterSingletonGeneric()
        {
            // Act
            container.RegisterSingleton<FakeUnityContainer>();

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterSingletonWithNameGeneric()
        {
            // Act
            container.RegisterSingleton<FakeUnityContainer>(name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeUnityContainer>(name));
        }

        [TestMethod]
        public void RegisterSingletonFromToGeneric()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<IUnityContainer, FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterSingletonFromToWithNameGeneric()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<IUnityContainer, FakeUnityContainer>(name));
        }

        #endregion


        #region Register Type

        [TestMethod]
        public void RegisterType()
        {
            // Act
            container.RegisterType(typeof(FakeUnityContainer));

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterTypeTo()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterTypeWithManager()
        {
            // Act
            container.RegisterType(typeof(FakeUnityContainer), (ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer), (ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeToWithManager()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), (ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeWithName()
        {
            // Act
            container.RegisterType(typeof(FakeUnityContainer), name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer), name));
        }

        [TestMethod]
        public void RegisterTypeToWithName()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name));
        }

        [TestMethod]
        public void RegisterTypeWithNameAndManager()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType(typeof(FakeUnityContainer), name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer), name, (ITypeLifetimeManager)manager));
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
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterSingletonWithName()
        {
            // Act
            container.RegisterSingleton(typeof(FakeUnityContainer), name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeUnityContainer), name));
        }

        [TestMethod]
        public void RegisterSingletonFromTo()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterSingletonFromToWithName()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer), name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeUnityContainer), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeUnityContainer), name));
        }

        #endregion
    }
}
