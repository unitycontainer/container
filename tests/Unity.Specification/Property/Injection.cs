using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Property.Injection;

namespace Property
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
