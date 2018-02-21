using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Specification
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
