using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{
    [TestClass]
    public class Hierarchical : Unity.Specification.Diagnostic.Hierarchical.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
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
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }
}
