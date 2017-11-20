using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Specification
{
    [TestClass]
    public class ContainerTestFixture : Container.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
