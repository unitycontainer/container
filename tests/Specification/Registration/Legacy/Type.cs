using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System.Collections.Generic;
using System.Linq;
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
        public void ChainRegistrations()
        {
            // Arrange
            var instance = new Service();

            Container.RegisterInstance(instance);
            Container.RegisterType<IService, Service>();

            // Act/Validate
            Assert.AreEqual(Container.Resolve<IService>(), instance);
        }


        [TestMethod]
        public void Registration_ShowsUpInRegistrationsSequence()
        {
            // Arrange
            Container.RegisterInstance(Name);
            Container.RegisterType<IService, Service>();
            Container.RegisterType<IService, Service>(Name);

            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>), Name);

            var registrations = (from r in Container.Registrations
                                 where r.RegisteredType == typeof(IService)
                                 select r).ToList();

            Assert.AreEqual(2, registrations.Count);

            Assert.IsTrue(registrations.Any(r => r.Name == null));
            Assert.IsTrue(registrations.Any(r => r.Name == Name));
        }

        [TestMethod]
        public void TypeMappingShowsUpInRegistrationsCorrectly()
        {
            // Arrange
            Container.RegisterInstance(Name);

            Container.RegisterType<IService, Service>();
            Container.RegisterType<IService, Service>(Name);

            var service = new Service();
            Container.RegisterInstance<IService>(service);
            Container.RegisterInstance<IService>(Name, service);

            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>));
            Container.RegisterType(typeof(IFoo<>), typeof(Foo<>), Name);

            var registration =
                (from r in Container.Registrations
                 where r.RegisteredType == typeof(IService)
                 select r).First();

#if BEHAVIOR_V4
            // Unity v4 incorrectly pointed mapped type
            Assert.AreSame(typeof(IService), registration.MappedToType);
#else
            Assert.AreSame(typeof(Service), registration.MappedToType);
#endif
        }

        [TestMethod]
        public void NonMappingRegistrationShowsUpInRegistrationsSequence()
        {
            Container.RegisterType<Service>();
            var registration = (from r in Container.Registrations
                                where r.RegisteredType == typeof(Service)
                                select r).First();

            Assert.AreSame(registration.RegisteredType, registration.MappedToType);
            Assert.IsNull(registration.Name);
        }

        [TestMethod]
        public void Registration_OfOpenGenericTypeShowsUpInRegistrationsSequence()
        {
            Container.RegisterType(typeof(IDictionary<,>), typeof(Dictionary<,>), "test");
            var registration = Container.Registrations.First(r => r.RegisteredType == typeof(IDictionary<,>));

            Assert.AreSame(typeof(Dictionary<,>), registration.MappedToType);
            Assert.AreEqual("test", registration.Name);
        }

        [TestMethod]
        public void Registration_InParentContainerAppearInChild()
        {
            Container.RegisterType<IService, Service>();
            var child = Container.CreateChildContainer();

            var registration =
                (from r in child.Registrations where r.RegisteredType == typeof(IService) select r).First();

            Assert.AreSame(typeof(Service), registration.MappedToType);
        }

        [TestMethod]
        public void Registration_InChildContainerDoNotAppearInParent()
        {
            var local = "local";
            var child = Container.CreateChildContainer()
                .RegisterType<IService, Service>(local);

            var childRegistration = child.Registrations
#if !UNITY_V4
                                         .Cast<IContainerRegistration>()
#endif
                                         .First(r => r.RegisteredType == typeof(IService) &&
                                                     r.Name == local);

            var parentRegistration = Container.Registrations
#if !UNITY_V4
                                              .Cast<IContainerRegistration>()
#endif
                                              .FirstOrDefault(r => r.RegisteredType == typeof(IService) &&
                                                                   r.Name == local);
            Assert.IsNull(parentRegistration);
            Assert.IsNotNull(childRegistration);
        }

        [TestMethod]
        public void Registration_InParentAndChildOnlyShowUpOnceInChild()
        {
            var child = Container.CreateChildContainer();
            child.RegisterType<IService, OtherService>(Name);

            var registrations = from r in child.Registrations
                                where r.RegisteredType == typeof(IService) & r.Name == Name
                                select r;

            var containerRegistrations = registrations.ToArray();

            Assert.AreEqual(1, containerRegistrations.Count());

            var childRegistration = containerRegistrations.First();
            Assert.AreSame(typeof(OtherService), childRegistration.MappedToType);
            Assert.AreEqual(Name, childRegistration.Name);
        }

        [TestMethod]
        public void DefaultLifetime()
        {
            // Arrange
            Container.RegisterType(typeof(object));

            // Act
            var registration = Container.Registrations.First(r => typeof(object) == r.RegisteredType);

            // Validate
#if BEHAVIOR_V4
            Assert.IsNull(registration.LifetimeManager);
#else
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(TransientLifetimeManager));
#endif
        }

        [TestMethod]
        public void CanSetLifetime()
        {
            // Arrange
            Container.RegisterType(typeof(object), new ContainerControlledLifetimeManager());

            // Act
            var registration = Container.Registrations.First(r => typeof(object) == r.RegisteredType);

            // Validate
            Assert.IsInstanceOfType(registration.LifetimeManager, typeof(ContainerControlledLifetimeManager));
        }

        [TestMethod]
        public void InvalidRegistration()
        {
            // Act
            Container.RegisterType<IService>();
        }

        [TestMethod]
        public void Singleton()
        {
            Container.RegisterType<IService, Service>(new ContainerControlledLifetimeManager());
            Container.RegisterType<IService, Service>(Name, new ContainerControlledLifetimeManager());

            // Act
            var anonymous = Container.Resolve<IService>();
            var named = Container.Resolve<IService>(Name);

            // Validate
            Assert.IsNotNull(anonymous);
            Assert.IsNotNull(named);
            Assert.AreNotSame(anonymous, named);
        }

    }
}
