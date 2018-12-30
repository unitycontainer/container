using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Method.Attribute;

namespace Method
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
