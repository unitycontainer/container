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
        public void TypeRegistrationShowsUpInRegistrations()
        {
            // Act
            Container.RegisterType<Service>();

            // Validate
            var registration = (from r in Container.Registrations
                                where r.RegisteredType == typeof(Service)
                                select r).First();

            Assert.AreSame(registration.RegisteredType, registration.MappedToType);
            Assert.IsNull(registration.Name);
        }
        
        [TestMethod]
        public void NamedRegistrationShowsUpInRegistrations()
        {
            // Act
            Container.RegisterType<Service>("name");

            // Validate
            var registration = (from r in Container.Registrations
                                where r.RegisteredType == typeof(Service)
                                select r).First();

            Assert.AreSame(registration.RegisteredType, registration.MappedToType);
            Assert.AreEqual(registration.Name, "name");
        }

        [TestMethod]
        public void MappingShowsUpInRegistrations()
        {
            // Act
            Container.RegisterType<IService, Service>();

            // Validate
            var registration =
                (from r in Container.Registrations where r.RegisteredType == typeof(IService) select r).First();

            Assert.AreSame(typeof(Service), registration.MappedToType);
        }

        [TestMethod]
        public void NamedMappingShowUpInRegistrations()
        {
            // Act
            Container.RegisterType<IService, Service>()
                     .RegisterType<IService, Service>("second");

            // Validate
            var registrations = (from r in Container.Registrations
                                 where r.RegisteredType == typeof(IService)
                                 select r).ToList();

            Assert.AreEqual(2, registrations.Count);

            Assert.IsTrue(registrations.Any(r => r.Name == null));
            Assert.IsTrue(registrations.Any(r => r.Name == "second"));
        }
    }
}
