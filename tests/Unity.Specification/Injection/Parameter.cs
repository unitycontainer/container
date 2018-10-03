using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Injection.Parameter;

namespace Injection
{
    [TestClass]
    public class Parameter : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
