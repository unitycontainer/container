using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container
{
    public partial class Resolution
    {
        [TestMethod, TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Optimized()
        {
            var instance = Container.Resolve(typeof(Service), Optimized);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod, TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Balanced()
        {
            var instance = Container.Resolve(typeof(Service), Balanced);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod, TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Singleton()
        {
            var instance = Container.Resolve(typeof(Service), Singleton);

            Assert.IsNotNull(instance);
            Assert.IsInstanceOfType(instance, typeof(Service));
        }

        [TestMethod, TestProperty(RESOLVE, REGISTERED)]
        public void Resolve_Registered_Singleton_Twice()
        {
            var instance1 = Container.Resolve(typeof(Service), Singleton);
            var instance2 = Container.Resolve(typeof(Service), Singleton);

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(Service));
            
            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof(Service));

            Assert.AreSame(instance1, instance2);
        }

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

        [TestMethod, TestProperty(RESOLVE, UNREGISTERED)]
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

        [Ignore("Requires further research")]
        [TestMethod, TestProperty(RESOLVE, UNREGISTERED)]
        public void Resolve_SharedService()
        {
            var instance1 = Container.Resolve<SharedService>();
            var instance2 = Container.Resolve<SharedService>();

            Assert.IsNotNull(instance1);
            Assert.IsInstanceOfType(instance1, typeof(SharedService));

            Assert.IsNotNull(instance2);
            Assert.IsInstanceOfType(instance2, typeof(SharedService));

            Assert.AreSame(instance1, instance2);
        }
    }
}
