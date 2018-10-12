using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Select.Method;

namespace Select
{
    [TestClass]
    public class Method : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
