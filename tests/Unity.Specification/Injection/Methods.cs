using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Injection.Methods;

namespace Injection
{
    [TestClass]
    public class Methods : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
