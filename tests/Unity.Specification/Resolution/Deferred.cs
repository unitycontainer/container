using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Resolution.Deferred;

namespace Resolution
{
    [TestClass]
    public class Deferred : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
