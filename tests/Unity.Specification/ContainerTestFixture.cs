using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity.Specification.Container;

namespace Unity.Specification.Tests
{
    [TestClass]
    public class ContainerTestFixture : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
