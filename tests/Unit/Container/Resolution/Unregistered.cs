using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container
{
    public partial class Resolution
    {
        [TestMethod, TestProperty(RESOLVE, UNREGISTERED)]
        public void Resolve_TransientService()
        {
            var instance1 = Container.Resolve<NonSharedService>();
            var instance2 = Container.Resolve<NonSharedService>();

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(NonSharedService));

            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof(NonSharedService));

            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod("Resolve Service"), TestProperty(RESOLVE, UNREGISTERED)]
        public void Resolve_Unregistered_Service()
        {
            var instance1 = Container.Resolve<Service>();
            var instance2 = Container.Resolve<Service>();

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(Service));

            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof(Service));

            Assert.AreNotSame(instance1, instance2);
        }

        [TestMethod("Resolve in child"), TestProperty(RESOLVE, UNREGISTERED)]
        public void Resolve_Unregistered_Child()
        {
            var container = Container.CreateChildContainer();
            var instance1 = container.Resolve<Service>();
            var instance2 = container.Resolve<Service>();

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(Service));

            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof(Service));

            Assert.AreNotSame(instance1, instance2);
        }
    }
}
