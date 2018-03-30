using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Specification.Tests
{
    [TestClass]
    public class ResolutionTestFixture : Resolution.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
