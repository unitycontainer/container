using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Property.Attribute;

namespace Property
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
