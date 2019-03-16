using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unity;
using Unity.Extension;

namespace Compiled
{

    [TestClass]
    public class Field : Unity.Specification.Diagnostic.Field.Validation.SpecificationTests
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
    public class Field : Unity.Specification.Diagnostic.Field.Validation.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer(UnityContainer.BuildStrategy.Resolved)
                .AddExtension(new Diagnostic());
        }
    }
}
