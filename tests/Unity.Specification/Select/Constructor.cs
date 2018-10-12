using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Select.Constructor;

namespace Select
{
    [TestClass]
    public class Constructor : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
