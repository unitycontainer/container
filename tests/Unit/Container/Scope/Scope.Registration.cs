using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void ExpandTest()
        {
            // Arrange
            var data1 = new RegistrationData(Name, new ContainerLifetimeManager(Name), new[] { TestTypes[0] });
            var data2 = new RegistrationData(Name, new ContainerLifetimeManager(Name), new[] { TestTypes[1] });

            // Act
            Scope.Register(in data1);
            Scope.Register(in data2);

            // Validate
            Assert.AreEqual(1, Scope.Names);
            Assert.AreEqual(5, Scope.Contracts);
        }

        [TestMethod]
        public void RegisterTypesTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);

            var data1 = new RegistrationData(null, manager1, TestTypes);
            var data2 = new RegistrationData(null, manager2, new[] { TestTypes[0], null });

            // Act
            Scope.Register(in data1);
            Scope.Register(in data2);

            // Validate
            Assert.AreEqual(0, Scope.Names);
            Assert.AreEqual(SizeTypes + 3, Scope.Contracts);
            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], ((TestScope)Scope).RegistryData[StartPosition + i].Contract.Type);
                Assert.AreSame(manager1,      ((TestScope)Scope).RegistryData[StartPosition + i].Manager);
            }

            Assert.AreEqual(TestTypes[0], ((TestScope)Scope).RegistryData[StartPosition].Contract.Type);
            Assert.AreSame(manager2, ((TestScope)Scope).RegistryData[StartPosition].Manager);
        }

        [Ignore]
        [TestMethod]
        public void RegisterTypesNameTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);

            var data1 = new RegistrationData(Name, manager1, TestTypes);
            var data2 = new RegistrationData(Name, manager2, new[] { TestTypes[0], null });

            // Act
            Scope.Register(in data1);
            Scope.Register(in data2);

            // Validate
            Assert.AreEqual(1, Scope.Names);
            Assert.AreEqual(SizeTypes + 3, Scope.Contracts);
            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], ((TestScope)Scope).RegistryData[StartPosition + i].Contract.Type);
                Assert.AreSame(manager1, ((TestScope)Scope).RegistryData[StartPosition + i].Manager);
            }

            Assert.AreEqual(TestTypes[0], ((TestScope)Scope).RegistryData[StartPosition].Contract.Type);
            Assert.AreSame(manager2, ((TestScope)Scope).RegistryData[StartPosition].Manager);
        }

        [Ignore]
        [TestMethod]
        public void RegisterTypesCollisionTest()
        {
            var manager = new ContainerLifetimeManager(Name);
            var data = new RegistrationData(null, manager, TestTypes);

            Scope.Register(in data);
            Scope.Register(in data);

            // Validate
            Assert.AreEqual(TestTypes.Length + 3, Scope.Contracts);
        }

        [Ignore]
        [TestMethod]
        public void RegisterTypesNamesCollisionTest()
        {
            // Act
            foreach (var name in TestNames)
            { 
                var manager = new ContainerLifetimeManager(name);
                var data = new RegistrationData(name, manager, TestTypes);
            
                Scope.Register(in data);
                Scope.Register(in data);
            }

            // Validate
            Assert.AreEqual(TestNames.Length, Scope.Names);
            Assert.AreEqual(TestNames.Length * TestTypes.Length + 3, Scope.Contracts);
        }

        [Ignore]
        [TestMethod]
        public void RegisterForLoopTest()
        {
            // Act
            foreach(var name in TestNames)
            {
                var manager = new ContainerLifetimeManager(name);
                var data = new RegistrationData(name, manager, TestTypes);

                Scope.Register(in data);
            }

            // Validate
            Assert.AreEqual(TestNames.Length * TestTypes.Length + 3, Scope.Contracts);
        }

        [Ignore]
        [TestMethod]
        public void RegisterParallelTest()
        {
            // Act
            var result = Parallel.ForEach(TestNames, (name) => 
            {
                var manager = new ContainerLifetimeManager(name);
                var data = new RegistrationData(name, manager, TestTypes);

                Scope.Register(in data);
            });

            // Validate
            Assert.AreEqual(TestNames.Length * TestTypes.Length + 3, Scope.Contracts);
        }
    }
}
