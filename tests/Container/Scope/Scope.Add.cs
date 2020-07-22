using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void EnumeratorTest()
        {
            var entries = Scope.ToArray();

            Assert.AreEqual(3, entries.Length);
        }

        [TestMethod]
        public void ExpandTest()
        {
            // Arrange
            var data1 = new RegistrationData(Name, new ContainerLifetimeManager(Name), new[] { TestTypes[0] });
            var data2 = new RegistrationData(Name, new ContainerLifetimeManager(Name), new[] { TestTypes[1] });

            // Act
            Scope.Add(in data1);
            Scope.Add(in data2);

            // Validate
            Assert.AreEqual(1, Scope.Names);
            Assert.AreEqual(5, Scope.Contracts);
        }


        [TestMethod]
        public void RegisterSameTypeTest()
        {
            // Arrange
            var data = new RegistrationData(null, new ContainerLifetimeManager(Name), new[] 
            { 
                typeof(Type), 
                typeof(Type), 
                typeof(string)
            });

            // Act
            Scope.Add(in data);

            // Validate
            //Assert.AreEqual(0, Scope.Names);
            //Assert.AreEqual(SizeTypes + 3, Scope.Contracts);
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
            Scope.Add(in data1);
            Scope.Add(in data2);

            // Validate
            Assert.AreEqual(0, Scope.Names);
            Assert.AreEqual(SizeTypes + 3, Scope.Contracts);
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
            Scope.Add(in data1);
            Scope.Add(in data2);

            // Validate
            Assert.AreEqual(1, Scope.Names);
            Assert.AreEqual(SizeTypes + 3, Scope.Contracts);

            var registrations = Scope.ToArray();

            for (var i = 1; i < SizeTypes; i++)
            {
                Assert.AreEqual(TestTypes[i], registrations[StartPosition - 1 + i].Contract.Type);
                Assert.AreSame(manager1,      registrations[StartPosition - 1 + i].Manager);
            }
        }

        [TestMethod]
        public void RegisterTypesCollisionTest()
        {
            var manager = new ContainerLifetimeManager(Name);
            var data = new RegistrationData(null, manager, TestTypes);

            Scope.Add(in data);
            Scope.Add(in data);

            // Validate
            Assert.AreEqual(TestTypes.Length + 3, Scope.Contracts);
        }

        [TestMethod]
        public void RegisterTypesNamesCollisionTest()
        {
            // Act
            foreach (var name in TestNames)
            { 
                var manager = new ContainerLifetimeManager(name);
                var data = new RegistrationData(name, manager, TestTypes);
            
                Scope.Add(in data);
                Scope.Add(in data);
            }

            // Validate
            Assert.AreEqual(TestNames.Length, Scope.Names);
            Assert.AreEqual(TestNames.Length * TestTypes.Length + 3, Scope.Contracts);
        }

        [TestMethod]
        public void RegisterForLoopTest()
        {
            // Act
            foreach(var name in TestNames)
            {
                var manager = new ContainerLifetimeManager(name);
                var data = new RegistrationData(name, manager, TestTypes);

                Scope.Add(in data);
            }

            // Validate
            Assert.AreEqual(TestNames.Length * TestTypes.Length + 3, Scope.Contracts);
        }

        [TestMethod]
        public void RegisterParallelTest()
        {
            // Act
            var result = Parallel.ForEach(TestNames, (name) => 
            {
                var manager = new ContainerLifetimeManager(name);
                var data = new RegistrationData(name, manager, TestTypes);

                Scope.Add(in data);
            });

            // Validate
            Assert.AreEqual(TestNames.Length * TestTypes.Length + 3, Scope.Contracts);
        }
    }
}
