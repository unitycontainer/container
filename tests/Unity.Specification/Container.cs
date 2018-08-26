using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Container;

namespace Specification.Tests
{
    [TestClass]
    public class Container : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
