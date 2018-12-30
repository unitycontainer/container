using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Constructor.Attribute;

namespace Constructor
{
    [TestClass]
    public class Attribute : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
