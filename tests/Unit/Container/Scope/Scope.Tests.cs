using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void RegisterTypeTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);

            // Act
            foreach (Type type in TestTypes) Scope.Register(type, manager1);
            Scope.Register(TestTypes[0], manager2);

            // Validate
            Assert.AreEqual(0,                            Scope.IdentityCount);
            Assert.AreEqual(StartPosition + SizeTypes - 1, Scope.RegistryCount);
            for (var i = 1; i < SizeTypes; i++)
            { 
                Assert.AreEqual(TestTypes[i], Scope.RegistryData[StartPosition + i].Type);
                Assert.AreSame(manager1, Scope.RegistryData[StartPosition + i].Manager);
            }

            Assert.AreEqual(TestTypes[0], Scope.RegistryData[StartPosition].Type);
            Assert.AreSame(manager2, Scope.RegistryData[StartPosition].Manager);
        }

        [TestMethod]
        public void RegisterTypesTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);
            var overrides = new[] { TestTypes[0], null };

            // Act
            Scope.Register(TestTypes, manager1);
            Scope.Register(overrides, manager2);

            // Validate
            Assert.AreEqual(0,                            Scope.IdentityCount);
            Assert.AreEqual(StartPosition + SizeTypes - 1, Scope.RegistryCount);
            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], Scope.RegistryData[StartPosition + i].Type);
                Assert.AreSame(manager1,      Scope.RegistryData[StartPosition + i].Manager);
            }

            Assert.AreEqual(TestTypes[0], Scope.RegistryData[StartPosition].Type);
            Assert.AreSame(manager2, Scope.RegistryData[StartPosition].Manager);
        }

        [TestMethod]
        public void RegisterTypeNameTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);

            // Act
            foreach (Type type in TestTypes) Scope.Register(type, Name, manager1);
            Scope.Register(TestTypes[0], Name, manager2);

            // Validate
            Assert.AreEqual(1,                            Scope.IdentityCount);
            Assert.AreEqual(StartPosition + SizeTypes - 1, Scope.RegistryCount);
            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], Scope.RegistryData[StartPosition + i].Type);
                Assert.AreSame(manager1, Scope.RegistryData[StartPosition + i].Manager);
                Assert.AreSame(Name, Scope.RegistryData[StartPosition + i].Name);
            }

            Assert.AreEqual(TestTypes[0], Scope.RegistryData[StartPosition].Type);
            Assert.AreSame(manager2, Scope.RegistryData[StartPosition].Manager);
        }

        [TestMethod]
        public void RegisterTypeNamesTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);

            // Act
            foreach (var name in TestNames) Scope.Register(typeof(string), name, manager1);
            Scope.Register(typeof(string), TestNames[0], manager2);

            // Validate
            Assert.AreEqual(SizeNames,                     Scope.IdentityCount);
            Assert.AreEqual(StartPosition + SizeNames - 1, Scope.RegistryCount);

            for (var i = 1; i < SizeNames; i++)
            {
                Assert.AreEqual(typeof(string), Scope.RegistryData[StartPosition + i].Type);
                Assert.AreEqual(TestNames[i],   Scope.RegistryData[StartPosition + i].Name);
                Assert.AreSame(manager1,        Scope.RegistryData[StartPosition + i].Manager);

                Assert.AreEqual(TestNames[i - 1], Scope.IdentityData[i].Name);
            }

            Assert.AreSame(manager2, Scope.RegistryData[StartPosition].Manager);
        }

        [TestMethod]
        public void RegisterTypesNameTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);
            var overrides = new[] { TestTypes[0], null };

            // Act
            Scope.Register(TestTypes, Name, manager1);
            Scope.Register(overrides, Name, manager2);

            // Validate
            Assert.AreEqual(1,                            Scope.IdentityCount);
            Assert.AreEqual(StartPosition + SizeTypes - 1, Scope.RegistryCount);
            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], Scope.RegistryData[StartPosition + i].Type);
                Assert.AreSame(manager1, Scope.RegistryData[StartPosition + i].Manager);
                Assert.AreSame(Name, Scope.RegistryData[StartPosition + i].Name);
            }

            Assert.AreEqual(TestTypes[0], Scope.RegistryData[StartPosition].Type);
            Assert.AreSame(manager2, Scope.RegistryData[StartPosition].Manager);
        }
    }
}
