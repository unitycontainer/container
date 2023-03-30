using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;
using Unity;
using Unity.Policy;

namespace Container.Extending
{
    [TestClass]
    public partial class ExtensionContextTests
    {
        ExtensionContext Context;   
        UnityContainer Container;

        [TestInitialize]
        public void TestInitialize()
        {
            Container = new UnityContainer();
            var mock = new MockContainerExtension();
            Container.AddExtension(mock);
            Context = mock.ExtensionContext;
        }

        [TestMethod]
        public void BaselineTest()
        {
            var unity = new UnityContainer();
            var extension = new MockContainerExtension();
            unity.AddExtension(extension);

            Assert.IsTrue(extension.InitializeWasCalled);
            Assert.IsNotNull(extension.ExtensionContext);
        }

        [TestMethod]
        public void ContainerTest()
        {
            // Validate
            Assert.IsNotNull(Context.Container);
            Assert.IsInstanceOfType(Context.Container, typeof(UnityContainer));
        }

        [TestMethod]
        public void PoliciesTest()
        {
            // Validate
            Assert.IsNotNull(Context.Policies);
            Assert.IsInstanceOfType(Context.Policies, typeof(IPolicyList));
        }
    }
}
