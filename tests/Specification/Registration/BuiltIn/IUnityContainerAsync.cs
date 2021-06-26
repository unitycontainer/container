using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public void IUnityContainerAsync()
        {
            var registration = Container.Registrations.ToList()[1];
            
            Assert.IsNotNull(registration);
            Assert.AreEqual(registration.RegisteredType, typeof(IUnityContainerAsync));
        }

        [TestMethod]
        public void AsyncContainerListsItselfAsRegistered()
        {
            Assert.IsTrue(Container.IsRegistered(typeof(IUnityContainerAsync)));
        }

        [TestMethod]
        public void AsyncContainerDoesNotListItselfUnderNonDefaultName()
        {
            Assert.IsFalse(Container.IsRegistered(typeof(IUnityContainerAsync), Name));
        }

        [TestMethod]
        public void AsyncContainerListsItselfAsRegisteredUsingGenericOverload()
        {
            Assert.IsTrue(Container.IsRegistered<IUnityContainerAsync>());
        }

        [TestMethod]
        public void AsyncContainerDoesNotListItselfUnderNonDefaultNameUsingGenericOverload()
        {
            Assert.IsFalse(Container.IsRegistered<IUnityContainerAsync>(Name));
        }
    }
}
