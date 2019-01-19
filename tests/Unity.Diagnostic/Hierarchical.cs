using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled.Hierarchical
{
    [TestClass]
    public class Container : Unity.Specification.Diagnostic.Hierarchical.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }
}

namespace Resolved.Hierarchical
{
    [TestClass]
    public class Container : Unity.Specification.Diagnostic.Hierarchical.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }
}
