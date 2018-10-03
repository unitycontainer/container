using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Injection.Method;

namespace Injection
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
