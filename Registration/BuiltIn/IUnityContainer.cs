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
        public void IUnityContainer()
        {
            var registration = Container.Registrations.First();
            
            Assert.IsNotNull(registration);
            Assert.AreEqual(registration.RegisteredType, typeof(IUnityContainer));
        }

        [TestMethod]
        public void ContainerListsItselfAsRegistered()
        {
            Assert.IsTrue(Container.IsRegistered(typeof(IUnityContainer)));
        }

        [TestMethod]
        public void ContainerDoesNotListItselfUnderNonDefaultName()
        {
            Assert.IsFalse(Container.IsRegistered(typeof(IUnityContainer), Name));
        }

        [TestMethod]
        public void ContainerListsItselfAsRegisteredUsingGenericOverload()
        {
            Assert.IsTrue(Container.IsRegistered<IUnityContainer>());
        }

        [TestMethod]
        public void ContainerDoesNotListItselfUnderNonDefaultNameUsingGenericOverload()
        {
            Assert.IsFalse(Container.IsRegistered<IUnityContainer>(Name));
        }
    }
}
