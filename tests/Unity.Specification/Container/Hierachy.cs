using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Container.Hierarchy;

namespace Container
{
    [TestClass]
    public class Hierarchy : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
