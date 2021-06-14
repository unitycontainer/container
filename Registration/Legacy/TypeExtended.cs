using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Regression;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
using Unity.Lifetime;
#endif

namespace Registration
{
    public partial class Legacy
    {
        [TestMethod]
        public void Type()
        {
            // Setup
            Container.RegisterType<object>();

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.IsNull(registration.Name);
#if BEHAVIOR_V4
            Assert.IsNull(registration.LifetimeManager);
#else
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(TransientLifetimeManager));
#endif
        }

        [TestMethod]
        public void Named()
        {
            // Setup
            Container.RegisterType<object>(Name);

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.AreEqual(Name, registration.Name);
#if BEHAVIOR_V4
            Assert.IsNull(registration.LifetimeManager);
#else
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(TransientLifetimeManager));
#endif
        }

        [TestMethod]
        public void WithLifetime()
        {
            // Setup
            Container.RegisterType<object>(new ContainerControlledLifetimeManager());

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.IsNull(registration.Name);
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void NamedWithLifetime()
        {
            // Setup
            Container.RegisterType<object>(Name, new ContainerControlledLifetimeManager());

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.AreEqual(Name, registration.Name);
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }


        [TestMethod]
        public void MappedType()
        {
            // Setup
            Container.RegisterType<IService, Service>();

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(IService) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.IsNull(registration.Name);
#if BEHAVIOR_V4
            Assert.IsNull(registration.LifetimeManager);
#else
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(TransientLifetimeManager));
#endif
            Assert.AreEqual(registration.RegisteredType, typeof(IService));
            Assert.AreEqual(registration.MappedToType, typeof(Service));
        }

        [TestMethod]
        public void MappedNamed()
        {
            // Setup
            Container.RegisterType<IService, Service>(Name);

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(IService) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.AreEqual(Name, registration.Name);
#if BEHAVIOR_V4
            Assert.IsNull(registration.LifetimeManager);
#else
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(TransientLifetimeManager));
#endif
            Assert.AreEqual(registration.RegisteredType, typeof(IService));
            Assert.AreEqual(registration.MappedToType, typeof(Service));
        }

        [TestMethod]
        public void MappedWithLifetime()
        {
            // Setup
            Container.RegisterType<IService, Service>(new ContainerControlledLifetimeManager());

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(IService) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.IsNull(registration.Name);
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.AreEqual(registration.RegisteredType, typeof(IService));
            Assert.AreEqual(registration.MappedToType, typeof(Service));
        }

        [TestMethod]
        public void MappedNamedWithLifetime()
        {
            // Setup
            Container.RegisterType<IService, Service>(Name, new ContainerControlledLifetimeManager());

            // Act
            var registration = Container.Registrations
                                        .FirstOrDefault(r => typeof(IService) == r.RegisteredType);

            // Validate
            Assert.IsNotNull(registration);
            Assert.AreEqual(Name, registration.Name);
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
            Assert.AreEqual(registration.RegisteredType, typeof(IService));
            Assert.AreEqual(registration.MappedToType, typeof(Service));
        }
    }
}
