using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{
    [TestClass]
    public class Hierarchical : Unity.Specification.Diagnostic.Hierarchical.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer().AddExtension(new ForceCompillation())
                                       .AddExtension(new Diagnostic());
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
            return new UnityContainer().AddExtension(new ForceActivation())
                                       .AddExtension(new Diagnostic());
        }
    }
}
