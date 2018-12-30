using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Container.Registrations;

namespace Container
{
    [TestClass]
    public class Registrations : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
