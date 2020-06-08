using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
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
            container.RegisterType<FakeIUC>();

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeIUC>());
        }

        [TestMethod]
        public void RegisterTypeToGeneric()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeIUC>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeIUC>());
        }

        [TestMethod]
        public void RegisterTypeGenericWithManager()
        {
            // Act
            container.RegisterType<FakeIUC>((ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);           // TODO: null vs array[]
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeIUC>((ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeToGenericWithManager()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeIUC>((ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeIUC>((ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeGenericWithName()
        {
            // Act
            container.RegisterType<FakeIUC>(name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeIUC>(name));
        }

        [TestMethod]
        public void RegisterTypeToGenericWithName()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType<IUnityContainer, FakeIUC>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeIUC>(name));
        }

        [TestMethod]
        public void RegisterTypeGenericWithNameAndManager()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType<FakeIUC>(name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeIUC>(name, (ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeToGenericWithNameAndManager()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType<IUnityContainer, FakeIUC>(name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeIUC>(name, (ITypeLifetimeManager)manager));
        }

        #endregion


        #region Generic Register Singleton

        [TestMethod]
        public void RegisterSingletonGeneric()
        {
            // Act
            container.RegisterSingleton<FakeIUC>();

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeIUC>());
        }

        [TestMethod]
        public void RegisterSingletonWithNameGeneric()
        {
            // Act
            container.RegisterSingleton<FakeIUC>(name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<FakeIUC>(name));
        }

        [TestMethod]
        public void RegisterSingletonFromToGeneric()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeIUC>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<IUnityContainer, FakeIUC>());
        }

        [TestMethod]
        public void RegisterSingletonFromToWithNameGeneric()
        {
            // Act
            container.RegisterSingleton<IUnityContainer, FakeIUC>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton<IUnityContainer, FakeIUC>(name));
        }

        #endregion


        #region Register Type

        [TestMethod]
        public void RegisterType()
        {
            // Act
            container.RegisterType(typeof(FakeIUC));

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeIUC)));
        }

        [TestMethod]
        public void RegisterTypeTo()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeIUC));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeIUC)));
        }

        [TestMethod]
        public void RegisterTypeWithManager()
        {
            // Act
            container.RegisterType(typeof(FakeIUC), (ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeIUC), (ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeToWithManager()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeIUC), (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeIUC), (ITypeLifetimeManager)manager));
        }

        [TestMethod]
        public void RegisterTypeWithName()
        {
            // Act
            container.RegisterType(typeof(FakeIUC), name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeIUC), name));
        }

        [TestMethod]
        public void RegisterTypeToWithName()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeIUC), name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsNull(container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeIUC), name));
        }

        [TestMethod]
        public void RegisterTypeWithNameAndManager()
        {
            // Arrange
            IUnityContainer unity = null;

            // Act
            container.RegisterType(typeof(FakeIUC), name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.AreSame(manager, container.LifetimeManager);
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeIUC), name, (ITypeLifetimeManager)manager));
        }

        #endregion


        #region Register Singleton

        [TestMethod]
        public void RegisterSingleton()
        {
            // Act
            container.RegisterSingleton(typeof(FakeIUC));

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeIUC)));
        }

        [TestMethod]
        public void RegisterSingletonWithName()
        {
            // Act
            container.RegisterSingleton(typeof(FakeIUC), name);

            // Validate
            Assert.IsNull(container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreEqual(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(FakeIUC), name));
        }

        [TestMethod]
        public void RegisterSingletonFromTo()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeIUC));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.IsNull(container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeIUC)));
        }

        [TestMethod]
        public void RegisterSingletonFromToWithName()
        {
            // Act
            container.RegisterSingleton(typeof(IUnityContainer), typeof(FakeIUC), name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Type);
            Assert.AreEqual(typeof(FakeIUC), container.MappedTo);
            Assert.AreSame(name, container.Name);
            Assert.IsInstanceOfType(container.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.IsNotNull(container.InjectionMembers);
            Assert.AreEqual(0, container.InjectionMembers.Length);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterSingleton(typeof(IUnityContainer), typeof(FakeIUC), name));
        }

        #endregion
    }
}
