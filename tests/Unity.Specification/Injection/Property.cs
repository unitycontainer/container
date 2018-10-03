using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Injection.Property;

namespace Injection
{
    [TestClass]
    public class Property : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
