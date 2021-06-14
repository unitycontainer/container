using Microsoft.VisualStudio.TestTools.UnitTesting;
using Regression;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Registration
{
    public partial class Collection
    {

        [TestMethod]
        public void RegistrationsInParentAppearInChild()
        {
            Container.RegisterType<IService, Service>();
            var child = Container.CreateChildContainer();

            var registration =
                (from r in child.Registrations where r.RegisteredType == typeof(IService) select r).First();

            Assert.AreSame(typeof(Service), registration.MappedToType);
        }

        [TestMethod]
        public void RegistrationsInChildDoNotAppearInParent()
        {
            var child = Container.CreateChildContainer()
                .RegisterType<IService, Service>("named");

            var childRegistration = child.Registrations
#if !UNITY_V4 && !BEHAVIOR_V5
                                         .Cast<IContainerRegistration>()
#endif
                                         .Where(r => r.RegisteredType == typeof(IService))
                                         .First();

            var parentRegistration = Container.Registrations
#if !UNITY_V4 && !BEHAVIOR_V5
                                              .Cast<IContainerRegistration>()
#endif
                                              .Where(r => r.RegisteredType == typeof(IService))
                                              .FirstOrDefault();

            Assert.IsNull(parentRegistration);
            Assert.IsNotNull(childRegistration);
        }

        [TestMethod]
        public void RegistrationInParentAndChildShowUpInChild()
        {
            Container.RegisterType<Service>();

            var child = Container.CreateChildContainer()
                                 .RegisterType<Service>();

            var registrations = from r in child.Registrations
                                where r.RegisteredType == typeof(Service)
                                select r;
#if BEHAVIOR_V4 || BEHAVIOR_V5
            Assert.AreEqual(1, registrations.Count());
#else
            Assert.AreEqual(2, registrations.Count());
#endif
            var childRegistration = registrations.First();
            Assert.AreSame(typeof(Service), childRegistration.MappedToType);
        }

        [TestMethod]
        public void NamedRegistrationInParentAndChildShowUpOnceInChild()
        {
            Container.RegisterType<Service>("one");

            var child = Container.CreateChildContainer()
                                 .RegisterType<Service>("one");

            var registrations = from r in child.Registrations
                                where r.RegisteredType == typeof(Service)
                                select r;

            Assert.AreEqual(1, registrations.Count());

            var childRegistration = registrations.First();
            Assert.AreSame(typeof(Service), childRegistration.MappedToType);
            Assert.AreEqual("one", childRegistration.Name);
        }


        [TestMethod]
        public void MapppingsInParentAndChildShowUpInChild()
        {
            Container.RegisterType<IService, Service>();

            var child = Container.CreateChildContainer()
                .RegisterType<IService, OtherService>();

            var registrations = from r in child.Registrations
                                where r.RegisteredType == typeof(IService)
                                select r;
#if BEHAVIOR_V4 || BEHAVIOR_V5
            Assert.AreEqual(1, registrations.Count());
#else
            Assert.AreEqual(2, registrations.Count());
#endif
            var childRegistration = registrations.First();
            Assert.AreSame(typeof(OtherService), childRegistration.MappedToType);
        }

        [TestMethod]
        public void NamedMappingInParentAndChildOnlyShowUpOnceInChild()
        {
            Container.RegisterType<IService, Service>("one");

            var child = Container.CreateChildContainer()
                .RegisterType<IService, OtherService>("one");

            var registrations = from r in child.Registrations
                                where r.RegisteredType == typeof(IService)
                                select r;

            Assert.AreEqual(1, registrations.Count());

            var childRegistration = registrations.First();
            Assert.AreSame(typeof(OtherService), childRegistration.MappedToType);
            Assert.AreEqual("one", childRegistration.Name);
        }






    }
}
