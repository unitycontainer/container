using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void IsRegisteredTest()
        {
            // Validate
            Assert.IsTrue(Scope.IsRegistered(typeof(IUnityContainer)));
            Assert.IsTrue(Scope.IsRegistered(typeof(IUnityContainerAsync)));
            Assert.IsTrue(Scope.IsRegistered(typeof(IServiceProvider)));

            Assert.IsFalse(Scope.IsRegistered(TestTypes[0]));
            Assert.IsFalse(Scope.IsRegistered(TestTypes[0], TestNames[0]));
        }

        [TestMethod]
        public void IsRegisteredTypeTest()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);
            var data    = new RegistrationData(null, manager, TestTypes);
            Scope.Register(in data);

            // Validate
            Assert.IsTrue(Scope.IsRegistered(typeof(IUnityContainer)));
            Assert.IsTrue(Scope.IsRegistered(typeof(IUnityContainerAsync)));
            Assert.IsTrue(Scope.IsRegistered(typeof(IServiceProvider)));

            Assert.IsTrue(Scope.IsRegistered(TestTypes[0]));
        }

        [TestMethod]
        public void IsRegisteredContractTest()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);

            foreach (var name in TestNames)
            { 
                var data    = new RegistrationData(name, manager, TestTypes);
                Scope.Register(in data);
            }

            // Validate
            Assert.IsTrue(Scope.IsRegistered(typeof(IUnityContainer)));
            Assert.IsTrue(Scope.IsRegistered(typeof(IUnityContainerAsync)));
            Assert.IsTrue(Scope.IsRegistered(typeof(IServiceProvider)));

            Assert.IsTrue(Scope.IsRegistered(TestTypes[0], TestNames[0]));
        }
    }
}
