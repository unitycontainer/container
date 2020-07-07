using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void RegisterTypesTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);

            var data1 = new RegistrationData(null, manager1, TestTypes);
            var data2 = new RegistrationData(null, manager2, new[] { TestTypes[0], null });

            // Act
            Scope.Register(ref data1);
            Scope.Register(ref data2);

            // Validate
            Assert.AreEqual(0,                            Scope.IdentityCount);
            Assert.AreEqual(StartPosition + SizeTypes - 1, Scope.RegistryCount);
            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], Scope.RegistryData[StartPosition + i].RegisteredType);
                Assert.AreSame(manager1,      Scope.RegistryData[StartPosition + i].LifetimeManager);
            }

            Assert.AreEqual(TestTypes[0], Scope.RegistryData[StartPosition].RegisteredType);
            Assert.AreSame(manager2, Scope.RegistryData[StartPosition].LifetimeManager);
        }

        [TestMethod]
        public void RegisterTypesNameTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);

            var data1 = new RegistrationData(Name, manager1, TestTypes);
            var data2 = new RegistrationData(Name, manager2, new[] { TestTypes[0], null });

            // Act
            Scope.Register(ref data1);
            Scope.Register(ref data2);

            // Validate
            Assert.AreEqual(1,                            Scope.IdentityCount);
            Assert.AreEqual(StartPosition + SizeTypes - 1, Scope.RegistryCount);
            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], Scope.RegistryData[StartPosition + i].RegisteredType);
                Assert.AreSame(manager1, Scope.RegistryData[StartPosition + i].LifetimeManager);
                Assert.AreSame(Name, Scope.RegistryData[StartPosition + i].Name);
            }

            Assert.AreEqual(TestTypes[0], Scope.RegistryData[StartPosition].RegisteredType);
            Assert.AreSame(manager2, Scope.RegistryData[StartPosition].LifetimeManager);
        }
    }
}
