using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Registrations.IsRegistered;

namespace Registrations
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
