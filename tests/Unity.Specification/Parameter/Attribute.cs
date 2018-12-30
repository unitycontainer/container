using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Parameter.Attribute;

namespace Parameter
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
