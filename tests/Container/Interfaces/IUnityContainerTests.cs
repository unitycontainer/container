using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Unity;

namespace Container.Interfaces
{
    [TestClass]
    public class IUnityContainerTests
    {
        protected UnityContainer Container;

        [TestInitialize]
        public virtual void InitializeTest() => Container = new UnityContainer();

        [TestMethod]
        public void Registrations()
        {
            var registrations = Container.Registrations.ToArray();

            Assert.AreEqual(3, registrations.Length);
        }
    }
}
