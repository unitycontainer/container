using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;

namespace Compiled
{

    [TestClass]
    public class Property : Unity.Specification.Diagnostic.Property.Validation.SpecificationTests
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
    public class Property : Unity.Specification.Diagnostic.Property.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }
}
