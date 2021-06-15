using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
#if UNITY_V4
using Microsoft.Practices.Unity;
#else
using Unity;
#endif

namespace Registration
{
    public partial class BuiltIn
    {
        [TestMethod]
        public void IServiceProvider()
        {
            var registration = Container.Registrations.ToList()[2];
            
            Assert.IsNotNull(registration);
            Assert.AreEqual(registration.RegisteredType, typeof(IServiceProvider));
        }

        [TestMethod]
        public void ServiceProviderListsItselfAsRegistered()
        {
            Assert.IsTrue(Container.IsRegistered(typeof(IServiceProvider)));
        }

        [TestMethod]
        public void ServiceProviderDoesNotListItselfUnderNonDefaultName()
        {
            Assert.IsFalse(Container.IsRegistered(typeof(IServiceProvider), Name));
        }

        [TestMethod]
        public void ServiceProviderListsItselfAsRegisteredUsingGenericOverload()
        {
            Assert.IsTrue(Container.IsRegistered<IServiceProvider>());
        }

        [TestMethod]
        public void ServiceProviderDoesNotListItselfUnderNonDefaultNameUsingGenericOverload()
        {
            Assert.IsFalse(Container.IsRegistered<IServiceProvider>(Name));
        }
    }
}
