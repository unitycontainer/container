using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Unity;
using Unity.Container;

namespace Container.Scope
{
    [TestClass]
    public partial class ScopeTests
    {
        protected UnityContainer Container;
        protected ContainerScope Scope;
        protected string Name = "0123456789";

        protected virtual UnityContainer GetContainer() => new UnityContainer();
            
        [TestInitialize]
        public virtual void InitializeTest()
        {
            Container = (UnityContainer)((IUnityContainer)GetContainer())
                .CreateChildContainer();

            Scope = Container._scope;
        }

        [TestMethod]
        public void Baseline()
        {
            var registrations = Scope.Registrations.ToArray();

            //Assert.IsNull(Scope.Parent);
            Assert.AreSame(Container, Scope.Container);
            Assert.AreEqual(3, registrations.Length);
            Assert.AreEqual(typeof(IUnityContainer), registrations[0].RegisteredType);
            Assert.AreEqual(typeof(IServiceProvider), registrations[1].RegisteredType);
            Assert.AreEqual(typeof(IUnityContainerAsync), registrations[2].RegisteredType);
        }
    }
}
