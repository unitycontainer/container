using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Lifetime;

namespace Unity.Benchmarks
{
    [TestClass]
    public class RegisterAPITests
    {
        #region Scaffolding

        protected static LifetimeManager Manager1;
        protected static LifetimeManager Manager2;
        protected static LifetimeManager Manager3;
        protected IUnityContainer Container;
        protected static string Name = "name";

        [TestInitialize]
        public void GlobalSetup()
        {
            Container = new UnityContainer();
            Manager1 = new ContainerControlledLifetimeManager();
            Manager2 = new ContainerControlledLifetimeManager();
            Manager3 = new ContainerControlledLifetimeManager();
        }

        #endregion


        [TestMethod]
        public void RegisterType()
        {
            Container.RegisterType(typeof(object), (ITypeLifetimeManager)Manager1);
            Container.RegisterType(typeof(object), Name, (ITypeLifetimeManager)Manager2);
            Container.RegisterType(typeof(string), "string", (ITypeLifetimeManager)Manager3);
        }

        [TestMethod]
        public void RegisterFactoryWithDescriptor()
        {
            Container.Register(new RegistrationDescriptor(typeof(object), null, (ITypeLifetimeManager)Manager1));
            Container.Register(new RegistrationDescriptor(new object(),   Name, (IInstanceLifetimeManager)Manager2));
            Container.Register(new RegistrationDescriptor(typeof(object), Name, (ITypeLifetimeManager)Manager3));
        }

    }
}
