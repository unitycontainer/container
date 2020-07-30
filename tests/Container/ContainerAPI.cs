using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Unity.Benchmarks
{
    [TestClass]
    public partial class ContainerAPI
    {
        protected IUnityContainer Container;

        [TestInitialize]
        public virtual void GlobalSetup() 
            => Container = new UnityContainer();

        [TestMethod]
        [TestCategory("new")]
        public void NewUnityContainer() 
            => Assert.IsNotNull( new UnityContainer());


        [TestMethod]
        [TestCategory("child")]
        public void CreateChildContainer() 
            => Assert.IsNotNull(Container.CreateChildContainer());


        [TestMethod]
        [TestCategory("registrations")]
        public void Registrations() 
            => Assert.IsInstanceOfType(Container.Registrations, typeof(IEnumerable<ContainerRegistration>));


        [TestMethod]
        [TestCategory("registrations")]
        public void RegistrationsToArray() 
            => Assert.IsInstanceOfType(Container.Registrations.ToArray(), typeof(ContainerRegistration[]));


        [TestMethod]
        [TestCategory("check")]
        public void IsRegistered() 
            => Assert.IsTrue(Container.IsRegistered(typeof(IUnityContainer), null));


        [TestMethod]
        [TestCategory("check")]
        public void IsNotRegistered() 
            => Assert.IsFalse(Container.IsRegistered(typeof(object), null));
    }
}
