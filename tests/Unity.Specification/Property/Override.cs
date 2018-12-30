using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Property.Overrides;

namespace Property
{
    [TestClass]
    public class Override : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}

