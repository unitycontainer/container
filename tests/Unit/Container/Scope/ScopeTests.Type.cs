using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void RegisterTypeTest()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);

            // Act
            Scope.Register(typeof(string), manager);
            Scope.Register(typeof(object), manager);
            Scope.Register(typeof(Type),   manager);
            Scope.Register(typeof(long),   manager);
            Scope.Register(typeof(int),    manager);

            // Validate

            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(object) == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(Type)   == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(long)   == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(int)    == r.RegisteredType && null  == r.Name));
        }

        [TestMethod]
        public void RegisterTypeCollisionTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);
            var manager3 = new ContainerLifetimeManager(Name);

            // Act
            Scope.Register(typeof(string), manager1);
            Scope.Register(typeof(string), manager2);
            Scope.Register(typeof(string), manager3);

            // Validate
            var registration = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && null == r.Name);

            Assert.AreSame(manager3, registration.LifetimeManager);
        }

    }
}
