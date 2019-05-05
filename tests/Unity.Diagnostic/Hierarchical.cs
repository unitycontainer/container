using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{
    [TestClass]
    public class Hierarchical : Unity.Specification.Diagnostic.Hierarchical.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceCompillation());
        }
    }
}

namespace Resolved
{
    [TestClass]
    public class Hierarchical : Unity.Specification.Diagnostic.Hierarchical.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceActivation());
        }
    }
}
