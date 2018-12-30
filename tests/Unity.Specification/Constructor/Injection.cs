using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Constructor.Injection;

namespace Constructor 
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
