using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Registration;

namespace Specification.Tests
{
    [TestClass]
    public class Registration : SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
