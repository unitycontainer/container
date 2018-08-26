using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Resolution;

namespace Specification.Tests
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
