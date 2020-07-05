using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    [TestClass]
    public class ScopeTests
    {
        protected UnityContainer Container;
        protected ContainerScope Scope;
        protected string Name = "0123456789";

        protected virtual UnityContainer GetContainer() => 
            (UnityContainer)((IUnityContainer)new UnityContainer()).CreateChildContainer();

        [TestInitialize]
        public virtual void InitializeTest()
        {
            Container = GetContainer();
            Scope = Container._scope;
        }

        [TestMethod]
        public void Baseline()
        {
            var registrations = Scope.Registrations.ToArray();

            //Assert.IsNull(Scope.Parent);
            Assert.AreSame(Container, Scope.Container);
            Assert.AreEqual(3, registrations.Length);
            Assert.AreEqual(typeof(IUnityContainer),      registrations[0].RegisteredType);
            Assert.AreEqual(typeof(IServiceProvider),     registrations[1].RegisteredType);
            Assert.AreEqual(typeof(IUnityContainerAsync), registrations[2].RegisteredType);
        }

        [TestMethod]
        public void RegisterTest()
        {
            // Arrange
            var manager = new ContainerLifetimeManager(Name);
            var Name1 = "123456789";
            var Name2 = "023456789";
            var Name3 = "013456789";
            var Name4 = "012456789";
            var Name5 = "012356789";

            // Act
            Scope.Register(typeof(string), null, manager);
            Scope.Register(typeof(string), Name1, manager);
            Scope.Register(typeof(object), null, manager);
            Scope.Register(typeof(object), Name2, manager);
            Scope.Register(typeof(Type),   null, manager);
            Scope.Register(typeof(Type),   Name3, manager);
            Scope.Register(typeof(long),   null, manager);
            Scope.Register(typeof(long),   Name4, manager);
            Scope.Register(typeof(int),    null, manager);
            Scope.Register(typeof(int),    Name5, manager);

            // Validate

            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(object) == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(Type)   == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(long)   == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(int)    == r.RegisteredType && null  == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(string) == r.RegisteredType && Name1 == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(object) == r.RegisteredType && Name2 == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(Type)   == r.RegisteredType && Name3 == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(long)   == r.RegisteredType && Name4 == r.Name));
            Assert.IsNotNull(Scope.Registrations.FirstOrDefault(r => typeof(int)    == r.RegisteredType && Name5 == r.Name));
        }

        [TestMethod]
        public void RegisterCollisionTest()
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
