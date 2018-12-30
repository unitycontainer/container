using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Field.Attribute;

namespace Field
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
