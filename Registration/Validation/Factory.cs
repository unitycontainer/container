using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Registration
{
    public partial class Validation
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterFactory_Null_Null_Null()
        {
            // Act
            Container.RegisterFactory(null, null, null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterFactory_Null_Null_Factory()
        {
            // Act
            Container.RegisterFactory(null, null, (c,t,n,o)=> null, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterFactory_Type_Null_Null()
        {
            // Act
            Container.RegisterFactory(typeof(object), null, null, null);
        }

        [TestMethod]
        public void RegisterFactory_WithOverride_DefaultLifetime()
        {
            // Arrange
            var value = new object();
            Container.RegisterFactory(typeof(object), null, (c, t, n, o) => value, null);

            // Act
            var registration = Container.Registrations.First(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(TransientLifetimeManager));
        }

        [TestMethod]
        public void RegisterFactory_WithOverride_CanSetLifetime()
        {
            // Arrange
            Container.RegisterFactory(typeof(object), null, (c, t, n, o) => null, new ContainerControlledLifetimeManager());

            // Act
            var registration = Container.Registrations.First(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void RegisterFactory_WithOverride_Type_Null_Factory()
        {
            // Arrange
            var value = new object();
            Container.RegisterFactory(typeof(object), null, (c, t, n, o) => value, null);

            // Act
            var instance = Container.Resolve<object>();

            // Validate
            Assert.AreSame(value, instance);
        }

        [TestMethod]
        public void RegisterFactory_WithOverride_Type_Name_Factory()
        {
            // Arrange
            var value = new object();
            Container.RegisterFactory(typeof(object), Name, (c, t, n, o) => value, null);

            // Act
            var instance = Container.Resolve<object>(Name);

            // Validate
            Assert.AreSame(value, instance);
        }
    }
}
