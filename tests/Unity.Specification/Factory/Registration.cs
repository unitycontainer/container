using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Specification.Factory.Registration;

namespace Factory
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
