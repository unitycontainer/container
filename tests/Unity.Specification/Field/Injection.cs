using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Field.Injection;

namespace Field
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
