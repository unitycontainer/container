using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Unity.Injection;
using Unity.Lifetime;

namespace Unity.Benchmarks
{
    [TestClass]
    public class ResolutionBenchmarksTests
    {
        protected IUnityContainer Container;

        [TestInitialize]
        public void GlobalSetup()
        {
            Container = new UnityContainer()
                .RegisterType(typeof(List<>), new InjectionConstructor())
                .CreateChildContainer();
        }

        [TestMethod]
        public void Baseline()
        {
        }

        [TestMethod]
        public void Resolve_IUnityContainer()
        {
            Assert.IsNotNull(Container.Resolve(typeof(IUnityContainer), null));
        }

        [TestMethod]
        public void Resolve_Object()
            => Container.Resolve(typeof(object), null);

        [TestMethod]
        public void Resolve_Object_Twice()
        {
            var instance1 = Container.Resolve(typeof(object), null);
            var instance2 = Container.Resolve(typeof(object), null);

            Assert.IsNotNull(instance1);
            Assert.IsNotNull(instance2);
        }

        [TestMethod]
        public void Resolve_Generic()
        {
            Assert.IsNotNull(Container.Resolve(typeof(List<int>), null));
        }
    }
}
