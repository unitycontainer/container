using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    [TestClass]
    public class ScopeTests
    {
        protected UnityContainer Container = new UnityContainer();
        protected ContainerScope Scope;

        [TestInitialize]
        public virtual void InitializeTest()
        {
            Scope = new ContainerScope(Container);
        }

        [TestMethod]
        public void Baseline()
        {
            var registrations = Scope.Registrations.ToArray();

            Assert.IsNull(Scope.Parent);
            Assert.AreSame(Container, Scope.Container);
            Assert.AreEqual(3, registrations.Length);
            Assert.AreEqual(typeof(IUnityContainer),      registrations[0].RegisteredType);
            Assert.AreEqual(typeof(IServiceProvider),     registrations[1].RegisteredType);
            Assert.AreEqual(typeof(IUnityContainerAsync), registrations[2].RegisteredType);
        }
    }
}
