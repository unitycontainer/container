using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Injection;
using Unity.Lifetime;
#endif

namespace Registration
{
    public partial class Validation
    {
        [TestMethod]
        public void RegisterInstance_IsNotNull()
        {
            // Arrange
            Container.RegisterInstance(typeof(IUnresolvable), null, Unresolvable.Create(string.Empty), new ContainerControlledLifetimeManager());
            
            // Validate
            Assert.IsNotNull(Container.Resolve<IUnresolvable>());
        }

        [TestMethod]
#if BEHAVIOR_V5
        [ExpectedException(typeof(InvalidOperationException))]
#else 
        [ExpectedException(typeof(ArgumentNullException))]
#endif
        public void RegisterInstance_ThrowsOnNullNull()
        {
            Container.RegisterInstance(null, null, null, new ContainerControlledLifetimeManager());
        }

        [TestMethod]
#if BEHAVIOR_V5
        [ExpectedException(typeof(InvalidOperationException))]
#else 
        [ExpectedException(typeof(ArgumentNullException))]
#endif
        public void RegisterInstance_Null_Null_Null()
        {
            // Act
            Container.RegisterInstance(null, null, null, null);
        }

        // Unity v4 did not support implicit instance lifetimes
        
#if !BEHAVIOR_V4

        [TestMethod]
        public void RegisterInstance_IsNull()
        {
            // Arrange
            Container.RegisterInstance(typeof(IService), null, null, new ContainerControlledLifetimeManager());

            // Validate
            Assert.IsNull(Container.Resolve<IService>());
        }

        [TestMethod]
        public void RegisterInstance_Null_Null_Instance()
        {
            // Arrange
            var value = new object();
            Container.RegisterInstance(null, null, value, null);

            // Act
            var instance = Container.Resolve<object>();

            // Validate
            Assert.AreSame(value, instance);
        }

        [TestMethod]
        public void RegisterInstance_Null_Name_Instance()
        {
            // Arrange
            var value = new object();
            Container.RegisterInstance(null, Name, value, null);

            // Act
            var instance = Container.Resolve<object>(Name);

            // Validate
            Assert.AreSame(value, instance);
        }

        [TestMethod]
        public void RegisterInstance_Type_Null_Null()
        {
            // Arrange
            Container.RegisterInstance(typeof(object), null, null, null);

            // Act
            var instance = Container.Resolve<object>();

            // Validate
            Assert.IsNull(instance);
        }

        [TestMethod]
        public void RegisterInstance_Type_Null_Instance()
        {
            // Arrange
            Container.RegisterInstance(typeof(object), null, Name, null);

            // Act
            var instance = Container.Resolve<object>();

            // Validate
            Assert.AreSame(Name, instance);
        }

        [TestMethod]
        public void RegisterInstance_Type_Name_Instance()
        {
            // Arrange
            Container.RegisterInstance(typeof(object), Name, Name, null);

            // Act
            var instance = Container.Resolve<object>(Name);

            // Validate
            Assert.AreSame(Name, instance);
        }

        [TestMethod]
        public void RegisterInstance_DefaultLifetime()
        {
            // Arrange
            var value = new object();
            Container.RegisterInstance(typeof(object), null, value, null);

            // Act
            var registration = Container.Registrations.First(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }
#endif

        [TestMethod]
        public void RegisterInstance_CanSetLifetime()
        {
            // Arrange
            var value = new object();
            Container.RegisterInstance(typeof(object), null, value, new ContainerControlledLifetimeManager());

            // Act
            var registration = Container.Registrations.First(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }
    }
}
