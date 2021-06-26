using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Registration
{
    public partial class Linq
    {
        [TestMethod]
        public void LegacyQuery_SuccessIfRegistered() 
        {
            var registration = Container.Registrations
                                        .FirstOrDefault(r => r.RegisteredType == typeof(IUnityContainer));

            Assert.IsNotNull(registration);
            Assert.IsNotNull(registration.RegisteredType);
        }

#if !UNITY_V4 && !UNITY_V5
        [TestMethod]
        public void LegacyQuery_FalsePositive()
        {
            var registration = Container.Registrations
                                        .FirstOrDefault(r => r.RegisteredType == typeof(Linq));

            Assert.IsNotNull(registration);
            Assert.IsNull(registration.RegisteredType);
        }

        [TestMethod]
        public void LegacyQuery_Corrected()
        {
            var registration = Container.Registrations
                                        .Cast<IContainerRegistration>() // Force boxing
                                        .FirstOrDefault(r => r.RegisteredType == typeof(Linq));

            Assert.IsNull(registration);
        }
#endif
        
        [TestMethod]
        public void RegistrationsToArray()
        {
            // Act
            var registrations = Container.Registrations.ToArray();

            // Validate
            Assert.IsNotNull(registrations);
#if UNITY_V5
            Assert.IsInstanceOfType(registrations, typeof(IContainerRegistration[]));
#else
            Assert.IsInstanceOfType(registrations, typeof(ContainerRegistration[]));
#endif
            Assert.AreNotEqual(0, registrations.Length);
        }
    }
}
