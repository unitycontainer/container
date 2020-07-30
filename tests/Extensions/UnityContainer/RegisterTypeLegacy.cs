using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;
using Unity.Abstractions.Tests;
using Unity.Injection;
using Unity.Lifetime;

namespace Extensions.Tests
{
    public partial class UnityContainerTests
    {
        #region Generic Register Type

        [TestMethod]
        public void RegisterType_Generic_Legacy()
        {
            // Act
            container.RegisterType<FakeUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.IsNotNull(container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterType_Manager_Generic_Legacy()
        {
            // Act
            container.RegisterType<FakeUnityContainer>((ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>((ITypeLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType<FakeUnityContainer>(FakeManager));
        }

        [TestMethod]
        public void RegisterType_Name_Generic_Legacy()
        {
            // Act
            container.RegisterType<FakeUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.IsNotNull(container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>(name));
        }

        [TestMethod]
        public void RegisterType_Name_Manager_Generic_Legacy()
        {
            // Act
            container.RegisterType<FakeUnityContainer>(name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<FakeUnityContainer>(name, (ITypeLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType<FakeUnityContainer>(name, FakeManager));
        }

        [TestMethod]
        public void RegisterTypeFrom_Generic_Legacy()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>();

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.IsNotNull(container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>());
        }

        [TestMethod]
        public void RegisterTypeFrom_Manager_Generic_Legacy()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>((ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>((ITypeLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType<IUnityContainer, FakeUnityContainer>(FakeManager));
        }

        [TestMethod]
        public void RegisterTypeFrom_Name_Generic_Legacy()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>(name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.IsNotNull(container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>(name));
        }

        [TestMethod]
        public void RegisterTypeFrom_Name_Manager_Generic_Legacy()
        {
            // Act
            container.RegisterType<IUnityContainer, FakeUnityContainer>(name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType<IUnityContainer, FakeUnityContainer>(name, (ITypeLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType<IUnityContainer, FakeUnityContainer>(name, FakeManager));
        }

        #endregion


        #region Register Type

        [TestMethod]
        public void RegisterType_Legacy()
        {
            // Act
            container.RegisterType(typeof(FakeUnityContainer));

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.IsNotNull(container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterType_Manager_Legacy()
        {
            // Act
            container.RegisterType(typeof(FakeUnityContainer), (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer), (ITypeLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer), (ITypeLifetimeManager)FakeManager));
        }

        [TestMethod]
        public void RegisterType_Name_Legacy()
        {
            // Act
            container.RegisterType(typeof(FakeUnityContainer), name);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.IsNotNull(container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer), name));
        }

        [TestMethod]
        public void RegisterType_Name_Manager_Legacy()
        {
            // Arrange
            var member = new InjectionConstructor();

            // Act
            container.RegisterType(typeof(FakeUnityContainer), name, (ITypeLifetimeManager)manager, member);

            // Validate
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(FakeUnityContainer), name, (ITypeLifetimeManager)manager, member));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType(typeof(FakeUnityContainer), name, (ITypeLifetimeManager)null, member));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType(typeof(FakeUnityContainer), name, FakeManager, member));
        }

        [TestMethod]
        public void RegisterType_ToFrom_Legacy()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer));

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.AreNotSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer)));
        }

        [TestMethod]
        public void RegisterType_Manager_ToFrom_Legacy()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.IsNull(container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), (ITypeLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), (ITypeLifetimeManager)null));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), FakeManager));
        }

        [TestMethod]
        public void RegisterType_Name_ToFrom_Legacy()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.AreNotSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name));
        }

        [TestMethod]
        public void RegisterType_Name_Manager_ToFrom_Legacy()
        {
            // Act
            container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name, (ITypeLifetimeManager)manager);

            // Validate
            Assert.AreEqual(typeof(IUnityContainer), container.Descriptor.RegisterAs.First());
            Assert.AreEqual(typeof(FakeUnityContainer), container.Descriptor.Type);
            Assert.AreSame(name, container.Descriptor.Name);
            Assert.AreSame(manager, container.Descriptor.Manager);
            Assert.ThrowsException<ArgumentNullException>(() => unity.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name, (ITypeLifetimeManager)manager));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name, (ITypeLifetimeManager)null));
            Assert.ThrowsException<ArgumentNullException>(() => container.RegisterType(typeof(IUnityContainer), typeof(FakeUnityContainer), name, FakeManager));
        }

        #endregion
    }
}
