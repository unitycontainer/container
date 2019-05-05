using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{

    [TestClass]
    public class Override : Unity.Specification.Diagnostic.Overrides.SpecificationTests
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
    public class Override : Unity.Specification.Diagnostic.Overrides.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(true).AddExtension(new ForceActivation());
        }
    }
}
