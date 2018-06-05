using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Tests.Issues
{
    [TestClass]
    public class FactoryResolve
    {
        [TestMethod]
        public void ResolveFactory_Normal()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<object>("key", new ContainerControlledLifetimeManager(), new InjectionFactory(c => new object()));
            container.Resolve<object>("key");
            container.Resolve<object>("key");
        }

        [TestMethod]
        public void ResolveFactory_ShowNotThrowOnResolve()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<object>("key", new ContainerControlledLifetimeManager(), new InjectionFactory(c => null));
            container.Resolve<object>("key");
            container.Resolve<object>("key");
        }

        [TestMethod]
        public void ResolveNonFactory_ShowNotThrowOnResolve()
        {
            IUnityContainer container = new UnityContainer();
            container.RegisterType<object>("key", new ContainerControlledLifetimeManager(), null);
            container.Resolve<object>("key");
            container.Resolve<object>("key");
        }
    }
}
