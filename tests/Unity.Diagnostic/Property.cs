using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;

namespace Compiled.Property
{

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Property.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Compiled)
                .AddExtension(new Diagnostic());
        }
    }
}

namespace Resolved.Property
{

    [TestClass]
    public class Validation : Unity.Specification.Diagnostic.Property.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }
}
