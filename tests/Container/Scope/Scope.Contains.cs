using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void ContainsTest()
        {
            // Validate
            Assert.IsTrue(Scope.Contains(typeof(IUnityContainer)));
            Assert.IsTrue(Scope.Contains(typeof(IUnityContainerAsync)));
            Assert.IsTrue(Scope.Contains(typeof(IServiceProvider)));

            Assert.IsFalse(Scope.Contains(TestTypes[0]));
            Assert.IsFalse(Scope.Contains(TestTypes[0], TestNames[0]));
        }

        [TestMethod]
        public void ContainsTypeTest()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);
            var data    = new RegistrationData(null, manager, TestTypes);
            Scope.Add(in data);

            // Validate
            Assert.IsTrue(Scope.Contains(typeof(IUnityContainer)));
            Assert.IsTrue(Scope.Contains(typeof(IUnityContainerAsync)));
            Assert.IsTrue(Scope.Contains(typeof(IServiceProvider)));

            Assert.IsTrue(Scope.Contains(TestTypes[0]));
        }

        [TestMethod]
        public void ContainsContractTest()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);

            foreach (var name in TestNames)
            { 
                var data    = new RegistrationData(name, manager, TestTypes);
                Scope.Add(in data);
            }

            // Validate
            Assert.IsTrue(Scope.Contains(typeof(IUnityContainer)));
            Assert.IsTrue(Scope.Contains(typeof(IUnityContainerAsync)));
            Assert.IsTrue(Scope.Contains(typeof(IServiceProvider)));

            Assert.IsTrue(Scope.Contains(TestTypes[0], TestNames[0]));
        }
    }
}
