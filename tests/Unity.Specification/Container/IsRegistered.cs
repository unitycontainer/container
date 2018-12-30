using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Container.IsRegistered;

namespace Container
{
    [TestClass]
    public class IsRegistered : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
