using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Method.Selection;

namespace Method
{
    [TestClass]
    public class Selection : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
