using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Registrations.Collection;

namespace Registrations
{
    [TestClass]
    public class Collection : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
