using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Select.Property;

namespace Select
{
    [TestClass]
    public class Property : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
