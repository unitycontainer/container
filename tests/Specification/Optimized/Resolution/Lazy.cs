using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Resolution.Lazy;

namespace Resolution
{
    [TestClass]
    public class Lazy : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
