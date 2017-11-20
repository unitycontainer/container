using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Specification
{
    [TestClass]
    public class InjectionTestFixture : Injection.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
