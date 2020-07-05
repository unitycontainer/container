using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity.Container;

namespace Container.Scope
{
    public partial class ScopeTests
    {
        [TestMethod]
        public void RegisterNameTest()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);

            // Act
            Scope.Register(typeof(string), Name, manager);
            Scope.Register(typeof(object), Name, manager);
            Scope.Register(typeof(Type),   Name, manager);
            Scope.Register(typeof(long),   Name, manager);
            Scope.Register(typeof(int),    Name, manager);

            // Validate
            var registration1 = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name == r.Name);
            var registration2 = Scope.Registrations.FirstOrDefault(r => typeof(object) == r.RegisteredType && Name == r.Name);
            var registration3 = Scope.Registrations.FirstOrDefault(r => typeof(Type)   == r.RegisteredType && Name == r.Name);
            var registration4 = Scope.Registrations.FirstOrDefault(r => typeof(long)   == r.RegisteredType && Name == r.Name);
            var registration5 = Scope.Registrations.FirstOrDefault(r => typeof(int)    == r.RegisteredType && Name == r.Name);

            Assert.IsTrue(typeof(string) == registration1.RegisteredType && Name == registration1.Name);
            Assert.IsTrue(typeof(object) == registration2.RegisteredType && Name == registration1.Name);
            Assert.IsTrue(typeof(Type)   == registration3.RegisteredType && Name == registration1.Name);
            Assert.IsTrue(typeof(long)   == registration4.RegisteredType && Name == registration1.Name);
            Assert.IsTrue(typeof(int)    == registration5.RegisteredType && Name == registration1.Name);
        }

        [TestMethod]
        public void RegisterTypeNames()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);
            var Name1 = "registration1";
            var Name2 = "registration2";
            var Name3 = "registration3";
            var Name4 = "registration4";
            var Name5 = "registration5";

            // Act
            Scope.Register(typeof(string), Name1, manager);
            Scope.Register(typeof(string), Name2, manager);
            Scope.Register(typeof(string), Name3, manager);
            Scope.Register(typeof(string), Name4, manager);
            Scope.Register(typeof(string), Name5, manager);

            // Validate
            var registration1 = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name1 == r.Name);
            var registration2 = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name2 == r.Name);
            var registration3 = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name3 == r.Name);
            var registration4 = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name4 == r.Name);
            var registration5 = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name5 == r.Name);

            Assert.IsTrue(typeof(string) == registration1.RegisteredType && Name1 == registration1.Name);
            Assert.IsTrue(typeof(string) == registration2.RegisteredType && Name2 == registration2.Name);
            Assert.IsTrue(typeof(string) == registration3.RegisteredType && Name3 == registration3.Name);
            Assert.IsTrue(typeof(string) == registration4.RegisteredType && Name4 == registration4.Name);
            Assert.IsTrue(typeof(string) == registration5.RegisteredType && Name5 == registration5.Name);
        }

        [TestMethod]
        public void RegisterNameCollisionTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);
            var manager3 = new ContainerLifetimeManager(Name);

            // Act
            Scope.Register(typeof(string), Name, manager1);
            Scope.Register(typeof(string), Name, manager2);
            Scope.Register(typeof(string), Name, manager3);

            // Validate
            var registration = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name == r.Name);

            Assert.AreSame(manager3, registration.LifetimeManager);
        }


        [TestMethod]
        public void RegisterNamevvTest()
        {
            // Arrange
            var manager1 = new ContainerLifetimeManager(Name);
            var manager2 = new ContainerLifetimeManager(Name);
            var manager3 = new ContainerLifetimeManager(Name);

            // Act
            Scope.Register(typeof(string), Name, manager1);
            Scope.Register(typeof(string), Name, manager2);
            Scope.Register(typeof(object), Name, manager1);
            Scope.Register(typeof(object), Name, manager2);
            Scope.Register(typeof(int),    Name, manager1);
            Scope.Register(typeof(int),    Name, manager2);
            Scope.Register(typeof(long),   Name, manager1);
            Scope.Register(typeof(long),   Name, manager2);

            // Validate
            var registration1 = Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name == r.Name);
            var registration2 = Scope.Registrations.FirstOrDefault(r => typeof(object) == r.RegisteredType && Name == r.Name);
            var registration3 = Scope.Registrations.FirstOrDefault(r => typeof(int)    == r.RegisteredType && Name == r.Name);
            var registration4 = Scope.Registrations.FirstOrDefault(r => typeof(long)   == r.RegisteredType && Name == r.Name);

            Assert.AreSame(manager2, registration1.LifetimeManager);
            Assert.AreSame(manager2, registration2.LifetimeManager);
            Assert.AreSame(manager2, registration3.LifetimeManager);
            Assert.AreSame(manager2, registration4.LifetimeManager);
        }

    }
}
