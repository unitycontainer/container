using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Extension;
using Unity.Policy;
using Unity;
using Unity.Container.Tests;

namespace Container.Extending
{
    [TestClass]
    public partial class ExtensionContextTests
    {
        ExtensionContext context;   
        UnityContainer container;

        [TestInitialize]
        public void TestInitialize()
        {
            container = new UnityContainer();
            var mock = new MockContainerExtension();
            container.AddExtension(mock);
            context = ((IMockConfiguration)mock).Context;
        }

        [TestMethod]
        public void BaselineTest()
        {
            var unity = new UnityContainer();
            var extension = new MockContainerExtension();
            unity.AddExtension(extension);

            Assert.IsTrue(extension.InitializeWasCalled);
            Assert.IsNotNull(((IMockConfiguration)extension).Context);
        }

        [TestMethod]
        public void ContainerTest()
        {
            // Validate
            Assert.IsNotNull(context.Container);
            Assert.IsInstanceOfType(context.Container, typeof(UnityContainer));
        }

        [Ignore]
        [TestMethod]
        public void PoliciesTest()
        {
            // Validate
            Assert.IsNotNull(context.Policies);
            Assert.IsInstanceOfType(context.Policies, typeof(IPolicyList));
        }

    }
}
