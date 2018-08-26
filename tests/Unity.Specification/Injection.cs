using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Injection;

namespace Specification.Tests
{
    [TestClass]
    public class Injection : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
