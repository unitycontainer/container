using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Container
{
    [TestClass]
    public class Managers : Unity.Specification.Lifetime.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation());
        }
    }
}
