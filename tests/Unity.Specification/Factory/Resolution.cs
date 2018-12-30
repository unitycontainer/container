using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Factory.Resolution;

namespace Factory
{
    [TestClass]
    public class Resolution : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
